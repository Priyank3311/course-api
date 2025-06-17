namespace Course.DataModel.Dtos.ResponseDTOs;

public class AuthResponseDto
{
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
