using AutoMapper;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.DataModel.Entities;
using Course.Repositories.UnitOfWork;
using Course.Services.Common;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Course.Services.Student;

public class StudentService(IMapper mapper, IUnitOfWork unitOfWork, IFirebaseNotificationService firebaseNotificationService) : IStudentService
{
    public async Task<CommonResponse<List<CourseResponseDto>>> GetAllCoursesAsync()
    {
        CommonResponse<List<CourseResponseDto>> response = new();
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
        CommonResponse<List<EnrollmentResponseDto>> response = new();
        try
        {
            List<Enrollment> enrollments = await unitOfWork.Enrollment.GetProjectedAsync(
                e => e.Userid == studentId,
                selector: e => e,
                include: q => q.Include(e => e.Course)
            );

            response.data = mapper.Map<List<EnrollmentResponseDto>>(enrollments);
            response.success_message = "Fetched your enrolled courses";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching courses: " + ex.Message);
            response.error_message = "Internal server error";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> EnrollAsync(int studentId, int courseId)
    {
        CommonResponse<bool> response = new();
        try
        {
            bool alreadyExists = (await unitOfWork.Enrollment
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

            User? student = await unitOfWork.User.GetByIdAsync(studentId);
            var course = await unitOfWork.Course.GetByIdAsync(courseId);
            string studentName = student?.Username ?? "Unknown Student";
            string courseName = course?.Coursename ?? "Unknown Course";

            List<Devicetoken> adminTokens = (await unitOfWork.DeviceToken
                                .FindAsync(a => a.User!.Role == "Admin"))
                                .GroupBy(t => t.Token)
                                .Select(g => g.First())
                                .ToList();


            foreach (Devicetoken token in adminTokens)
            {
                try
                {
                    await firebaseNotificationService.SendToTokenAsync(token.Token,
                        "New Enrollment",
                        $"{studentName} has enrolled in course name: {courseName}");
                }
                catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
                {
                    Console.WriteLine($" Token unregistered, removing from DB: {token.Token}");
                    await unitOfWork.DeviceToken.DeleteAsync(token);
                }
            }

            response.data = true;
            response.success_message = "Enrolled successfully";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enrolling student: {ex.Message}");
            response.data = false;
            response.error_message = "Internal server error";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> MarkCompletedAsync(int studentId, int courseId)
    {
        CommonResponse<bool> response = new();
        try
        {
            Enrollment? enrollment = (await unitOfWork.Enrollment
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
            Console.WriteLine($"Error marking course as completed: {ex.Message}");
            response.data = false;
            response.error_message = "Internal server error";
        }
        return response;
    }

    public async Task<CommonResponse<StudentProfileDto?>> GetProfileAsync(int studentId)
    {
        CommonResponse<StudentProfileDto?> response = new();
        try
        {
            var user = await unitOfWork.User.GetByIdAsync(studentId);
            if (user == null)
            {
                response.error_message = "Student not found";
                return response;
            }

            IEnumerable<Enrollment> enrollments = await unitOfWork.Enrollment.FindAsync(e => e.Userid == studentId);
            List<int> courseIds = enrollments.Select(e => e.Courseid).ToList();
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
            Console.WriteLine($"Error fetching student profile: {ex.Message}");
            response.error_message = "Internal server error";
        }
        return response;
    }
}
