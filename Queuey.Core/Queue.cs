using Nito.AsyncEx;
using Noppes.Queuey.Core.Models;
using Noppes.Queuey.Core.Repositories;

namespace Noppes.Queuey.Core;

public interface IQueue
{
    Task<IList<string>> EnqueueAsync(IList<EnqueueItem> items);

    Task<IList<DequeueItem>> DequeueAsync(int limit, TimeSpan? visibilityDelay = null);

    Task<bool> AcknowledgeAsync(string id);
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

    public async Task<IList<string>> EnqueueAsync(IList<EnqueueItem> items)
    {
        using var _ = await _lock.LockAsync();

        foreach (var item in items)
            item.VisibleWhen = item.VisibleWhen.ToUniversalTime();

        var ids = await _repository.AddAsync(items);
        return ids;
    }

    public async Task<IList<DequeueItem>> DequeueAsync(int limit, TimeSpan? visibilityDelay = null)
    {
        using var _ = await _lock.LockAsync();

        var items = await _repository.GetAsync(limit, visibilityDelay ?? TimeSpan.Zero);
        return items;
    }

    public async Task<bool> AcknowledgeAsync(string id)
    {
        using var _ = await _lock.LockAsync();

        var couldBeFound = await _repository.MoveToHistoryAsync(id);
        return couldBeFound;
    }
}
