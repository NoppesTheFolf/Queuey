using Noppes.Queuey.Core.Repositories;
using System.Collections.Concurrent;

namespace Noppes.Queuey.Core;

public interface IQueueProvider
{
    IQueue Get(string name);
}

internal class QueueProvider : IQueueProvider
{
    private readonly IQueueItemRepositoryProvider _provider;
    private readonly ConcurrentDictionary<string, IQueue> _queues;

    public QueueProvider(IQueueItemRepositoryProvider provider)
    {
        _provider = provider;
        _queues = new ConcurrentDictionary<string, IQueue>();
    }

    public IQueue Get(string name)
    {
        var queue = _queues.GetOrAdd(name, x =>
        {
            var repository = _provider.Get(x);
            var createdQueue = new Queue(repository);

            return createdQueue;
        });

        return queue;
    }
}
