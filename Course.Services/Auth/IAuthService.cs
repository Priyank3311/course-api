using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;

namespace Course.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<CommonResponse<string>> RefreshTokenAsync(RefreshRequestDto dto);
    Task<CommonResponse<string>> SaveDeviceToken(DeviceTokenDto dto);
}
