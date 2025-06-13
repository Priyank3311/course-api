using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;

namespace Course.Services.Courses;

public interface ICourseService
{
    Task<CommonResponse<List<CourseResponseDto>>> GetPagedAsync(string? search, string? dept, int page, int size);
    Task<CommonResponse<CourseResponseDto>> CreateAsync(CourseRequestDto dto);
    Task<CommonResponse<bool>> UpdateAsync(int id, CourseRequestDto dto);
    Task<CommonResponse<bool>> DeleteAsync(int id);
    Task<CommonResponse<List<StudentInCourseDto>>> GetEnrolledStudentsAsync(int courseId);
    
}
