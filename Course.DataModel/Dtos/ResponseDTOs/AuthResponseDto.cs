namespace Course.DataModel.Dtos.ResponseDTOs;

public class AuthResponseDto
{
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Token { get; set; } = null!;
}
