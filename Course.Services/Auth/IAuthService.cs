using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;

namespace Course.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto);
}
