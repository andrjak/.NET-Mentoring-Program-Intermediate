using REST.Data;
using REST.Services;

namespace REST.Helpers;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ItemService>();
        services.AddScoped<CategoryService>();

        return services;
    }

    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationContext>();

        return services;
    }

}
