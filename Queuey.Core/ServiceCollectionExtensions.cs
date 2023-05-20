using Microsoft.Extensions.DependencyInjection;

namespace Noppes.Queuey.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCore(this IServiceCollection services)
    {
        services.AddSingleton<IQueueProvider, QueueProvider>();

        return services;
    }
}
