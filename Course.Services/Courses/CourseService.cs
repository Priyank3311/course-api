using System.Linq.Expressions;
using AutoMapper;
using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.DataModel.Entities;
using Course.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Course.Services.Courses;

public class CourseService : ICourseService
{
    private readonly IMapper mapper;

    private readonly IUnitOfWork unitOfWork;


    public CourseService(IMapper _mapper, IUnitOfWork _unitOfWork)
    {
        mapper = _mapper;
        unitOfWork = _unitOfWork;
    }

    public async Task<CommonResponse<List<CourseResponseDto>>> GetPagedAsync(string? search, string? dept, int page, int size)
    {
        var response = new CommonResponse<List<CourseResponseDto>>();
        try
        {
            Expression<Func<DataModel.Entities.Course, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(search) || !string.IsNullOrWhiteSpace(dept))
            {
                filter = c =>
                    (string.IsNullOrWhiteSpace(search) || c.Coursename.ToLower().Trim().Contains(search.ToLower().Trim())) &&
                    (string.IsNullOrWhiteSpace(dept) || c.Department.ToLower().Trim().Contains(dept.ToLower().Trim()));
            }

            var courses = await unitOfWork.Course.GetDataAsync(filter);
            var TotalCount = courses.Count;
            var pagedCourses = courses
                                    .Skip((page - 1) * size)
                                    .Take(size)
                                    .ToList();
            var dtos = mapper.Map<List<CourseResponseDto>>(pagedCourses);
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
        var response = new CommonResponse<CourseResponseDto>();
        try
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "CourseRequestDto cannot be null");

            var course = mapper.Map<DataModel.Entities.Course>(dto);
            await unitOfWork.Course.AddAsync(course);

            response.data = mapper.Map<CourseResponseDto>(course);
            response.success_message = "Course created successfully";
        }
        catch (Exception ex)
        {
            response.error_message = $"Failed to create course: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> UpdateAsync(int id, CourseRequestDto dto)
    {
        var response = new CommonResponse<bool>();
        try
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "CourseRequestDto cannot be null");

            var course = await unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {id} not found");

            mapper.Map(dto, course);
            await unitOfWork.Course.UpdateAsync(course);

            response.data = true;
            response.success_message = "Course updated successfully";
        }
        catch (Exception ex)
        {
            response.data = false;
            response.error_message = $"Failed to update course: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<bool>> DeleteAsync(int id)
    {
        var response = new CommonResponse<bool>();
        try
        {
            var deleted = await unitOfWork.Course.DeleteIfAsync(
                c => c.Id == id,
                include: q => q.Include(c => c.Enrollments),
                condition: c => !c.Enrollments!.Any()
            );

            response.data = deleted;
            response.success_message = deleted ? "Course deleted successfully" : "Course cannot be deleted";
        }
        catch (Exception ex)
        {
            response.data = false;
            response.error_message = $"Failed to delete course: {ex.Message}";
        }
        return response;
    }

    public async Task<CommonResponse<List<StudentInCourseDto>>> GetEnrolledStudentsAsync(int courseId)
    {
        var response = new CommonResponse<List<StudentInCourseDto>>();
        try
        {
            var students = await unitOfWork.Enrollment.GetProjectedAsync(
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
        var response = new CommonResponse<CourseResponseDto>();
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
            response.error_message = $"Failed to fetch course: {ex.Message}";
        }
        return response;
    }

}
