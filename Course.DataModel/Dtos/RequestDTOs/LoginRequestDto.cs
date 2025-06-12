using FluentValidation;

namespace Course.DataModel.Dtos.RequestDTOs;

public class LoginRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
public class LoginRequestValidation : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidation()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}