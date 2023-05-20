using Nito.AsyncEx;
using Noppes.Queuey.Core.Models;
using Noppes.Queuey.Core.Repositories;

namespace Noppes.Queuey.Core;

public interface IQueue
{
    Task EnqueueAsync(ICollection<EnqueueItem> items);

    Task<ICollection<DequeueItem>?> DequeueAsync(int limit, TimeSpan? visibilityDelay = null);

    Task AcknowledgeAsync(string id);
}

internal class Queue : IQueue
{
    private readonly IQueueItemRepository _repository;
    private readonly AsyncLock _lock;

    public Queue(IQueueItemRepository repository)
    {
        _repository = repository;
        _lock = new AsyncLock();
    }

    public async Task EnqueueAsync(ICollection<EnqueueItem> items)
    {
        using var _ = await _lock.LockAsync();

        foreach (var item in items)
            item.VisibleWhen = item.VisibleWhen.ToUniversalTime();

        await _repository.Add(items);
    }

    public async Task<ICollection<DequeueItem>?> DequeueAsync(int limit, TimeSpan? visibilityDelay = null)
    {
        using var _ = await _lock.LockAsync();

        var item = await _repository.Get(limit, visibilityDelay ?? TimeSpan.Zero);
        return item;
    }

    public async Task AcknowledgeAsync(string id)
    {
        using var _ = await _lock.LockAsync();

        await _repository.MoveToHistoryAsync(id);
    }
}
