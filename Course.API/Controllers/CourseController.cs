using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CourseController(ICourseService _courseService) : ControllerBase
{
    [HttpGet("Course")]
    public async Task<IActionResult> GetPagedAsync(string? search, string? dept, int page = 1, int size = 10)
    {
        try
        {
            CommonResponse<List<CourseResponseDto>> courses = await _courseService.GetPagedAsync(search, dept, page, size);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest($"Internal server error");
        }
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateAsync([FromForm] CourseRequestDto dto)
    {
        try
        {
            CommonResponse<CourseResponseDto> course = await _courseService.CreateAsync(dto);
            return Ok(course);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest($"Internal server error");
        }
    }

    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateAsync(int id, [FromForm] CourseRequestDto dto)
    {
        try
        {
            CommonResponse<bool> response = await _courseService.UpdateAsync(id, dto);

            if (!string.IsNullOrEmpty(response.error_message))
                return BadRequest(response);

            if (response.data == false)
                return NotFound(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest($"Internal server error");
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            CommonResponse<CourseResponseDto> response = await _courseService.GetByIdAsync(id);

            if (!string.IsNullOrEmpty(response.error_message))
                return NotFound(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest($"Internal server error");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            CommonResponse<bool> response = await _courseService.DeleteAsync(id);

            if (!string.IsNullOrEmpty(response.error_message))
                return BadRequest(response);

            if (response.data == false)
                return BadRequest(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest($"Internal server error");
        }
    }
    [HttpGet("{courseId}/students")]
    public async Task<IActionResult> GetEnrolledStudentsAsync(int courseId)
    {
        try
        {
            CommonResponse<List<StudentInCourseDto>> students = await _courseService.GetEnrolledStudentsAsync(courseId);
            return Ok(students);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest($"Internal server error");
        }
    }
}
