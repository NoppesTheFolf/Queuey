using MongoDB.Bson;
using MongoDB.Driver;
using Noppes.Queuey.Core.Models;
using Noppes.Queuey.Core.Repositories;
using Noppes.Queuey.MongoDB.Entities;
using Noppes.Queuey.MongoDB.Extensions;

namespace Noppes.Queuey.MongoDB;

internal class QueueItemRepository : IQueueItemRepository
{
    private readonly IMongoCollection<QueueItemEntity> _collection;
    private readonly IMongoCollection<QueueItemEntity> _historyCollection;

    public QueueItemRepository(IMongoCollection<QueueItemEntity> collection, IMongoCollection<QueueItemEntity> historyCollection)
    {
        _collection = collection;
        _historyCollection = historyCollection;
    }

    public async Task Add(ICollection<EnqueueItem> items)
    {
        var entities = items.Select(item => new QueueItemEntity
        {
            Priority = item.Priority,
            VisibleWhen = item.VisibleWhen,
            Message = item.Message,
            DequeuedWhen = new List<DateTime>(),
            DequeueCount = 0,
            CreatedWhen = DateTime.UtcNow,
            AcknowledgedWhen = null
        }).ToList();

        await _collection.InsertManyAsync(entities);
    }

    public async Task<IList<DequeueItem>?> Get(int limit, TimeSpan visibilityDelay)
    {
        var entities = await GetItemsToDequeue(limit);
        if (!entities.Any())
            return Array.Empty<DequeueItem>();

        await MarkItemsAsDequeuedAsync(entities, visibilityDelay);

        var items = entities.Select(x => new DequeueItem
        {
            Id = x.Id.ToString(),
            Priority = x.Priority,
            DequeueCount = x.DequeueCount + 1,
            Message = x.Message
        }).ToList();
        return items;
    }

    private async Task<List<QueueItemEntity>> GetItemsToDequeue(int limit)
    {
        // Receive the queue item with the highest priority that is visible
        var filter = Builders<QueueItemEntity>.Filter.Where(x => DateTime.UtcNow >= x.VisibleWhen);
        var sort = Builders<QueueItemEntity>.Sort.Descending(x => x.Priority).Ascending(x => x.CreatedWhen);
        var cursor = await _collection.FindAsync(filter, new FindOptions<QueueItemEntity>
        {
            Sort = sort,
            Limit = limit
        });
        var entities = await cursor.ToListAsync();

        return entities;
    }

    private async Task MarkItemsAsDequeuedAsync(ICollection<QueueItemEntity> entities, TimeSpan visibilityDelay)
    {
        var now = DateTime.UtcNow;
        var filter = Builders<QueueItemEntity>.Filter.In(x => x.Id, entities.Select(y => y.Id));
        var update = Builders<QueueItemEntity>.Update
            .Set(x => x.VisibleWhen, now.Add(visibilityDelay)) // Increase the visibility
            .AddToSet(x => x.DequeuedWhen, now) // Keep track of when the items were dequeued
            .Inc(x => x.DequeueCount, 1); // Increment the number of times the items have been dequeued
        await _collection.UpdateManyAsync(filter, update);
    }

    public async Task MoveToHistoryAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        var idFilter = Builders<QueueItemEntity>.Filter.Eq(x => x.Id, objectId);

        var entity = await _collection.FindOneOrDefaultAsync(idFilter);
        if (entity == null)
            return;

        var historyEntity = await _historyCollection.FindOneOrDefaultAsync(idFilter);
        if (historyEntity == null)
        {
            entity.AcknowledgedWhen = DateTime.UtcNow;
            await _historyCollection.InsertOneAsync(entity);
        }

        await _collection.DeleteOneAsync(idFilter);
    }
}
