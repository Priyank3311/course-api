using System.Security.Claims;
using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.Services.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentController(IStudentService _studentService) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirst("user_id")?.Value!);

    [HttpGet("available-courses")]
    public async Task<IActionResult> GetAvailableCourses()
    {
        return Ok(await _studentService.GetAllCoursesAsync());
    }

    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses()
    {
        return Ok(await _studentService.GetMyCoursesAsync(GetUserId()));
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll(EnrollRequestDto dto)
    {
        CommonResponse<bool> response = await _studentService.EnrollAsync(GetUserId(), dto.CourseId);

        if (!string.IsNullOrEmpty(response.error_message))
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete(EnrollRequestDto dto)
    {
        CommonResponse<bool> response = await _studentService.MarkCompletedAsync(GetUserId(), dto.CourseId);

        if (!string.IsNullOrEmpty(response.error_message))
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await _studentService.GetProfileAsync(GetUserId()));
    }
}
