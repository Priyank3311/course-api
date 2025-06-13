using Course.DataModel.Dtos.ResponseDTOs;

namespace Course.Services.Student;

public interface IStudentService
{
    Task<CommonResponse<List<CourseResponseDto>>> GetAllCoursesAsync();
    Task<CommonResponse<List<EnrollmentResponseDto>>> GetMyCoursesAsync(int studentId);
    Task<CommonResponse<bool>> EnrollAsync(int studentId, int courseId);
    Task<CommonResponse<bool>> MarkCompletedAsync(int studentId, int courseId);
    Task<CommonResponse<StudentProfileDto?>> GetProfileAsync(int studentId);
}
