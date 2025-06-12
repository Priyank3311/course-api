using FluentValidation;

namespace Course.DataModel.Dtos.RequestDTOs;

public class RegisterRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Student";
}

public class RegisterRequestValidation : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidation()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Role).NotEmpty();
        }
    }