using AutoMapper;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.DataModel.Entities;
using Course.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Course.Services.Student;

public class StudentService : IStudentService
{
    private readonly IMapper mapper;

    private readonly IUnitOfWork unitOfWork;


    public StudentService(IMapper _mapper, IUnitOfWork _unitOfWork)
    {
        mapper = _mapper;
        unitOfWork = _unitOfWork;
    }
    public async Task<CommonResponse<List<CourseResponseDto>>> GetAllCoursesAsync()
    {
        var response = new CommonResponse<List<CourseResponseDto>>();
        try
        {
            var courses = await unitOfWork.Course.GetAllAsync();
            response.data = mapper.Map<List<CourseResponseDto>>(courses);
            response.success_message = "Fetched all available courses";
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to fetch courses: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<List<EnrollmentResponseDto>>> GetMyCoursesAsync(int studentId)
    {
        var response = new CommonResponse<List<EnrollmentResponseDto>>();
        try
        {
            var enrollments = await unitOfWork.Enrollment.GetProjectedAsync(
                e => e.Userid == studentId,
                selector: e => e,
                include: q => q.Include(e => e.Course)
            );

            response.data = mapper.Map<List<EnrollmentResponseDto>>(enrollments);
            response.success_message = "Fetched your enrolled courses";
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to fetch your courses: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> EnrollAsync(int studentId, int courseId)
    {
        var response = new CommonResponse<bool>();
        try
        {
            var alreadyExists = (await unitOfWork.Enrollment
                .FindAsync(e => e.Userid == studentId && e.Courseid == courseId))
                .Any();

            if (alreadyExists)
            {
                response.data = false;
                response.error_message = "You are already enrolled in this course.";
                return response;
            }

            await unitOfWork.Enrollment.AddAsync(new Enrollment
            {
                Userid = studentId,
                Courseid = courseId,
                Iscompleted = false
            });

            response.data = true;
            response.success_message = "Enrolled successfully";
        }
        catch (Exception ex)
        {
            response.data = false;
            response.error_message = $"Enrollment failed: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> MarkCompletedAsync(int studentId, int courseId)
    {
        var response = new CommonResponse<bool>();
        try
        {
            var enrollment = (await unitOfWork.Enrollment
                .FindAsync(e => e.Userid == studentId && e.Courseid == courseId))
                .FirstOrDefault();

            if (enrollment == null)
            {
                response.data = false;
                response.error_message = "Enrollment not found";
                return response;
            }

            enrollment.Iscompleted = true;
            await unitOfWork.Enrollment.UpdateAsync(enrollment);

            response.data = true;
            response.success_message = "Marked as completed";
        }
        catch (Exception ex)
        {
            response.data = false;
            response.error_message = $"Failed to mark as completed: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<StudentProfileDto?>> GetProfileAsync(int studentId)
    {
        var response = new CommonResponse<StudentProfileDto?>();
        try
        {
            var user = await unitOfWork.User.GetByIdAsync(studentId);
            if (user == null)
            {
                response.error_message = "Student not found";
                return response;
            }

            var enrollments = await unitOfWork.Enrollment.FindAsync(e => e.Userid == studentId);
            var courseIds = enrollments.Select(e => e.Courseid).ToList();
            var courses = await unitOfWork.Course.FindAsync(c => courseIds.Contains(c.Id));

            response.data = new StudentProfileDto
            {
                Username = user.Username,
                TotalCredits = courses.Sum(c => c.Credits),
                EnrolledCourses = enrollments.Count()
            };
            response.success_message = "Profile data fetched";
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to get profile: {ex.Message}";
        }
        return response;
    }
}
