using Microsoft.Extensions.DependencyInjection;
using Noppes.Queuey.Core.Repositories;

namespace Noppes.Queuey.MongoDB;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMongoDB(this IServiceCollection services, string connectionString, string databaseName, string historicDatabaseName)
    {
        var repositoryProvider = new QueueItemRepositoryProvider(connectionString, databaseName, historicDatabaseName);
        services.AddSingleton<IQueueItemRepositoryProvider>(repositoryProvider);

        return services;
    }
}