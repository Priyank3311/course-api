using Course.DataModel.Dtos.RequestDTOs;
using Course.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers;

[ApiController]
[Route("api/Auth/")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequestDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest("Username already exists.");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }

    }
}
