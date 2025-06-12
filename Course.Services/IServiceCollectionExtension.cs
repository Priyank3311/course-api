using Course.Repositories.UnitOfWork;
using Course.Services.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Course.Services;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
