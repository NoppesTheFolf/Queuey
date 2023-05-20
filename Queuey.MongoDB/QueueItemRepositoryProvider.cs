using MongoDB.Driver;
using Noppes.Queuey.Core.Repositories;
using Noppes.Queuey.MongoDB.Entities;
using System.Collections.Concurrent;

namespace Noppes.Queuey.MongoDB;

internal class QueueItemRepositoryProvider : IQueueItemRepositoryProvider
{
    private readonly IMongoDatabase _database;
    private readonly IMongoDatabase _historicDatabase;
    private readonly ConcurrentDictionary<(IMongoDatabase, string), IMongoCollection<QueueItemEntity>> _queues;

    public QueueItemRepositoryProvider(string connectionString, string databaseName, string historicDatabaseName)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        var mongoClient = new MongoClient(settings);
        _database = mongoClient.GetDatabase(databaseName);
        _historicDatabase = mongoClient.GetDatabase(historicDatabaseName);

        _queues = new ConcurrentDictionary<(IMongoDatabase, string), IMongoCollection<QueueItemEntity>>();
    }

    private IMongoCollection<QueueItemEntity> GetQueue(IMongoDatabase database, string name, bool addIndex)
    {
        var collection = _queues.GetOrAdd((database, name), _ => CreateQueue(database, name, addIndex));

        return collection;
    }

    private static IMongoCollection<QueueItemEntity> CreateQueue(IMongoDatabase database, string name, bool addIndex)
    {
        var collection = database.GetCollection<QueueItemEntity>(name);

        if (addIndex)
        {
            var indexKeysDefinition = Builders<QueueItemEntity>.IndexKeys
                .Descending(y => y.Priority)
                .Ascending(y => y.CreatedWhen);
            collection.Indexes.CreateOneAsync(new CreateIndexModel<QueueItemEntity>(indexKeysDefinition));
        }

        return collection;
    }

    public IQueueItemRepository Get(string name)
    {
        var queue = GetQueue(_database, name, true);
        var historicQueue = GetQueue(_historicDatabase, name, false);
        var repository = new QueueItemRepository(queue, historicQueue);

        return repository;
    }
}
