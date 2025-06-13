using Course.Repositories.UnitOfWork;
using Course.Services.Auth;
using Course.Services.Courses;
using Course.Services.Student;
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
        return services;
    }
}
