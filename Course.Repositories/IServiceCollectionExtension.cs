using Microsoft.Extensions.DependencyInjection;

namespace Course.Repositories;

public static class IServiceCollectionExtension
{
     public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services;
        }
}
