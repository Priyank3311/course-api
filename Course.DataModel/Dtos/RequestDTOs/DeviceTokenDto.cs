namespace Course.DataModel.Dtos.RequestDTOs;

public class DeviceTokenDto
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
}
