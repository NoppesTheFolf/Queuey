namespace Noppes.Queuey.Core.Repositories;

public interface IQueueItemRepositoryProvider
{
    IQueueItemRepository Get(string name);
}
