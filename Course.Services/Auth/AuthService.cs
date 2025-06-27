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

public class AuthService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration config) : IAuthService
{
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
        User user = mapper.Map<User>(dto);
        user.Passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await unitOfWork.User.AddAsync(user);
        return GenerateTokens(user);

    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "LoginRequestDto cannot be null");
        }
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ArgumentException("Username and Password cannot be empty");
        }

        User? user = (await unitOfWork.User.FindAsync(x => x.Username == dto.Username)).FirstOrDefault();
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Passwordhash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        return GenerateTokens(user);
    }

    // private AuthResponseDto GenerateToken(User user)
    // {
    //     var claims = new[]
    //     {
    //             new Claim(ClaimTypes.Name, user.Username),
    //             new Claim(ClaimTypes.Role, user.Role),
    //             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    //         };

    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //     var token = new JwtSecurityToken(
    //         issuer: config["Jwt:Issuer"],
    //         audience: config["Jwt:Audience"],
    //         claims: claims,
    //         expires: DateTime.UtcNow.AddHours(2),
    //         signingCredentials: creds
    //     );

    //     return new AuthResponseDto
    //     {
    //         Username = user.Username,
    //         Role = user.Role,
    //         Token = new JwtSecurityTokenHandler().WriteToken(token)
    //     };
    // }

    public async Task<CommonResponse<string>> RefreshTokenAsync(RefreshRequestDto dto)
    {
        CommonResponse<string> response = new();
        try
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken token = handler.ReadJwtToken(dto.refreshToken);

            TokenValidationParameters parameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };

            handler.ValidateToken(dto.refreshToken, parameters, out var validatedToken);

            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            string? username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                response.error_message = "Invalid refresh token: username not found in claims.";
                return response;
            }


            User? user = (await unitOfWork.User.FindAsync(u => u.Username == username)).FirstOrDefault();
            if (user == null)
            {
                response.error_message = "User not found.";
                return response;
            }

            // Generate new access token
            string newAccessToken = GenerateJwtToken(user, DateTime.UtcNow.AddMinutes(30));

            response.data = newAccessToken;
            response.success_message = "Access token refreshed successfully.";
        }
        catch (SecurityTokenException ste)
        {
            response.error_message = "Refresh token validation failed: " + ste.Message;
        }
        catch (Exception ex)
        {
            response.error_message = "Unexpected error: " + ex.Message;
        }

        return response;
    }

    private AuthResponseDto GenerateTokens(User user)
    {
        try
        {
            return new AuthResponseDto
            {
                Username = user.Username,
                Role = user.Role,
                AccessToken = GenerateJwtToken(user, DateTime.UtcNow.AddMinutes(30)),
                RefreshToken = GenerateJwtToken(user, DateTime.UtcNow.AddHours(2))
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error generating tokens: " + ex.Message);
        }

    }

    private string GenerateJwtToken(User user, DateTime expiresIn)
    {
        try
        {
            Claim[] claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("user_id", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: expiresIn,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error generating JWT token: " + ex.Message);
        }

    }


    public async Task<CommonResponse<string>> SaveDeviceToken(DeviceTokenDto dto)
    {
        CommonResponse<string> response = new();
        try
        {
            // var token = new Devicetoken
            // {
            //     Userid = dto.UserId,
            //     Token = dto.Token
            // };

            // await unitOfWork.DeviceToken.AddAsync(token);
            IEnumerable<Devicetoken> existing = await unitOfWork.DeviceToken
                                .FindAsync(t => t.Userid == dto.UserId && t.Token == dto.Token);

            if (!existing.Any())
            {
                await unitOfWork.DeviceToken.AddAsync(new Devicetoken
                {
                    Userid = dto.UserId,
                    Token = dto.Token
                });
            }

            response.data = "Device token saved successfully.";
            response.success_message = "Device token saved successfully.";
        }
        catch (Exception ex)
        {
            response.error_message = $"An error occurred while saving device token: {ex.Message}";
        }

        return response;
    }

}
