using System.Linq.Expressions;
using AutoMapper;
using Course.Common;
using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.DataModel.Entities;
using Course.Repositories.UnitOfWork;
using Course.Services.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Course.Services.Courses;

public class CourseService(IMapper mapper, IUnitOfWork unitOfWork, IHubContext<CourseHub> _hubContext, ICommonService _commonService) : ICourseService
{

    public async Task<CommonResponse<List<CourseResponseDto>>> GetPagedAsync(string? search, string? dept, int page, int size)
    {
        CommonResponse<List<CourseResponseDto>> response = new();
        try
        {
            Expression<Func<DataModel.Entities.Course, bool>>? filter = null;

            // if (!string.IsNullOrWhiteSpace(search) || !string.IsNullOrWhiteSpace(dept))
            // {
            //     filter = c =>
            //         (string.IsNullOrWhiteSpace(search) || c.Coursename.ToLower().Trim().Contains(search.ToLower().Trim())) &&
            //         (string.IsNullOrWhiteSpace(dept) || c.Department.ToLower().Trim().Contains(dept.ToLower().Trim()));
            // }
            if (!string.IsNullOrWhiteSpace(search))
            {
                filter = c =>
                    c.Coursename.ToLower().Trim().Contains(search.ToLower().Trim()) ||
                    c.Department.ToLower().Trim().Contains(search.ToLower().Trim());
            }

            IEnumerable<DataModel.Entities.Course> courses = await unitOfWork.Course.GetDataAsync(filter);
            var TotalCount = courses.Count();
            var pagedCourses = courses
                                    .Skip((page - 1) * size)
                                    .Take(size)
                                    .ToList();
            List<CourseResponseDto> dtos = mapper.Map<List<CourseResponseDto>>(pagedCourses);
            foreach (var dto in dtos)
            {
                dto.TotalCount = TotalCount;
            }
            response.data = dtos;
            response.success_message = "Course list fetched successfully";
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to fetch course list: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<CourseResponseDto>> CreateAsync(CourseRequestDto dto)
    {
        CommonResponse<CourseResponseDto> response = new();
        try
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "CourseRequestDto cannot be null");

            DataModel.Entities.Course course = mapper.Map<DataModel.Entities.Course>(dto);
            if (dto.CourseImage != null && dto.CourseImage.Length > 0)
            {
                course.ImagePath = await _commonService.UploadImageAsync(dto.CourseImage, "course-images");
            }
            await unitOfWork.Course.AddAsync(course);

            response.data = mapper.Map<CourseResponseDto>(course);
            response.success_message = "Course created successfully";
            await _hubContext.Clients.All.SendAsync("NewCourseAdded", response.data);
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to create course: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> UpdateAsync(int id, CourseRequestDto dto)
    {
        CommonResponse<bool> response = new();
        try
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "CourseRequestDto cannot be null");

            var course = await unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {id} not found");

            mapper.Map(dto, course);
            if (dto.CourseImage != null && dto.CourseImage.Length > 0)
            {


                course.ImagePath = _commonService.UploadImageAsync(dto.CourseImage, "course-images").Result;
            }
            await unitOfWork.Course.UpdateAsync(course);

            response.data = true;
            response.success_message = "Course updated successfully";
            // var updatedCourse = mapper.Map<CourseResponseDto>(course);
            // await _hubContext.Clients.All.SendAsync("CourseUpdated", updatedCourse);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            response.data = false;
            response.error_message = $"Internal server error";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> DeleteAsync(int id)
    {
        CommonResponse<bool> response = new();
        try
        {
            bool deleted = await unitOfWork.Course.DeleteIfAsync(
                c => c.Id == id,
                include: q => q.Include(c => c.Enrollments),
                condition: c => !c.Enrollments!.Any()
            );

            response.data = deleted;
            response.success_message = deleted ? "Course deleted successfully" : "Course cannot be deleted";
            // await _hubContext.Clients.All.SendAsync("CourseDeleted", id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting course: {ex.Message}");
            response.data = false;
            response.error_message = $"Failed to delete course";
        }
        return response;
    }

    public async Task<CommonResponse<List<StudentInCourseDto>>> GetEnrolledStudentsAsync(int courseId)
    {
        CommonResponse<List<StudentInCourseDto>> response = new();
        try
        {
            List<StudentInCourseDto> students = await unitOfWork.Enrollment.GetProjectedAsync(
                e => e.Courseid == courseId,
                selector: e => new StudentInCourseDto
                {
                    StudentId = e.Userid,
                    Username = e.User!.Username,
                    IsCompleted = (bool)e.Iscompleted
                },
                include: q => q.Include(e => e.User)
            );

            response.data = students;
            response.success_message = "Enrolled students fetched successfully";
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to fetch students: {ex.Message}";
        }

        return response;
    }
    public async Task<CommonResponse<CourseResponseDto>> GetByIdAsync(int id)
    {
        CommonResponse<CourseResponseDto> response = new();
        try
        {
            var course = await unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                response.data = null;
                response.error_message = $"Course with ID {id} not found.";
                return response;
            }

            response.data = mapper.Map<CourseResponseDto>(course);
            response.success_message = "Course fetched successfully.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching course: {ex.Message}");
            response.error_message = "Internal server error";
        }
        return response;
    }

}
