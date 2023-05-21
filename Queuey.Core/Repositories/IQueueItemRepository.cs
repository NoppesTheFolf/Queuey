using Noppes.Queuey.Core.Models;

namespace Noppes.Queuey.Core.Repositories;

public interface IQueueItemRepository
{
    Task<IList<string>> AddAsync(ICollection<EnqueueItem> items);

    Task<IList<DequeueItem>> GetAsync(int limit, TimeSpan visibilityDelay);

    Task<bool> MoveToHistoryAsync(string id);
}
