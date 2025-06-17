using Course.DataModel.Dtos.RequestDTOs;
using Course.Repositories.UnitOfWork;
using Course.Services.Auth;
using Course.Services.Courses;
using Course.Services.Student;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Course.Services;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.RegisterRequestValidatorDependencies();
        return services;
    }

    public static void RegisterRequestValidatorDependencies(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CourseRequestDto>, CourseRequestValidation>();
        services.AddScoped<IValidator<EnrollRequestDto>, EnrollRequestValidation>();
        services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidation>();
        services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidation>();
    }
}
