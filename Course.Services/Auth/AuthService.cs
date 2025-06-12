using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.DataModel.Entities;
using Course.Repositories.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Course.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IMapper mapper;

    private readonly IUnitOfWork unitOfWork;

    private readonly IConfiguration config;

    public AuthService(IMapper _mapper, IUnitOfWork _unitOfWork, IConfiguration _config)
    {
        mapper = _mapper;
        unitOfWork = _unitOfWork;
        config = _config;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "RegisterRequestDto cannot be null");
        }
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ArgumentException("Username and Password cannot be empty");
        }
        if ((await unitOfWork.User.FindAsync(x => x.Username == dto.Username)).Any())
        {
            throw new InvalidOperationException("Username already exists");
        }
        var user = mapper.Map<User>(dto);
        user.Passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await unitOfWork.User.AddAsync(user);
        return GenerateToken(user);

    }

    private AuthResponseDto GenerateToken(User user)
    {
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new AuthResponseDto
        {
            Username = user.Username,
            Role = user.Role,
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }

}
