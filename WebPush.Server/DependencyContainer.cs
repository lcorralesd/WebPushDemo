using WebPush.Server.Services;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyContainer
{
    public static IServiceCollection AddWebPushServerServices(this IServiceCollection services)
    {

        services.AddScoped<WebPushService>();
        services.AddHttpClient<WebPushService>();

        return services;
    }
}
