using Course.DataModel.Dtos.RequestDTOs;
using Course.Services.Courses;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("api/Course")]
    public async Task<IActionResult> GetPagedAsync(string? search, string? dept, int page = 1, int size = 10)
    {
        try
        {
            var courses = await _courseService.GetPagedAsync(search, dept, page, size);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CourseRequestDto dto)
    {
        try
        {
            var course = await _courseService.CreateAsync(dto);
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CourseRequestDto dto)
    {
        try
        {
            var response = await _courseService.UpdateAsync(id, dto);

            if (!string.IsNullOrEmpty(response.error_message))
                return BadRequest(response);

            if (response.data == false)
                return NotFound(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _courseService.GetByIdAsync(id);

            if (!string.IsNullOrEmpty(response.error_message))
                return NotFound(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var response = await _courseService.DeleteAsync(id);

            if (!string.IsNullOrEmpty(response.error_message))
                return BadRequest(response);

            if (response.data == false)
                return BadRequest(response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
    [HttpGet("{courseId}/students")]
    public async Task<IActionResult> GetEnrolledStudentsAsync(int courseId)
    {
        try
        {
            var students = await _courseService.GetEnrolledStudentsAsync(courseId);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
}
