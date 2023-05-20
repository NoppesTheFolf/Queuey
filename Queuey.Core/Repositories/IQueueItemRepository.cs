using Noppes.Queuey.Core.Models;

namespace Noppes.Queuey.Core.Repositories;

public interface IQueueItemRepository
{
    Task Add(ICollection<EnqueueItem> items);

    Task<IList<DequeueItem>?> Get(int limit, TimeSpan visibilityDelay);

    Task MoveToHistoryAsync(string id);
}
