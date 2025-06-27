using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers;


[ApiController]
[Route("api/Auth/")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        CommonResponse<AuthResponseDto> response = new();
        try
        {
            AuthResponseDto? result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest("Username already exists.");
            Response.Cookies.Append("jwtToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            response.data = result;
            response.success_message = "Registered successfully";
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            response.error_message = ex.Message;
            return BadRequest(response);
        }
        catch(ArgumentNullException ex)
        {
            response.error_message = ex.Message;
            return BadRequest(response);
        }
        catch (ArgumentException ex)
        {
            response.error_message = ex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            response.error_message = $"Internal server error";
            return StatusCode(500, response);
        }

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        CommonResponse<AuthResponseDto> response = new();

        try
        {
            AuthResponseDto? result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Invalid username or password.");
            Response.Cookies.Append("jwtToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            response.data = result;
            response.success_message = "Login successful";
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            response.error_message = ex.Message;
            return Unauthorized(response);
        }
        catch (ArgumentException ex)
        {
            response.error_message = ex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            response.error_message = $"Internal server error";
            return StatusCode(500, response);
        }
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
    {
        CommonResponse<string> result = await _authService.RefreshTokenAsync(dto);

        if (!string.IsNullOrEmpty(result.error_message))
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("device-token")]
    public async Task<IActionResult> SaveDeviceToken([FromBody] DeviceTokenDto dto)
    {
        CommonResponse<string> response = await _authService.SaveDeviceToken(dto);
        return Ok(response);
    }

}
