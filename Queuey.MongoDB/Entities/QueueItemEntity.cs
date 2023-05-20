using MongoDB.Bson;

namespace Noppes.Queuey.MongoDB.Entities;

internal class QueueItemEntity
{
    /// <summary>
    /// Unique identifier. Has no meaning other than that.
    /// </summary>
    public ObjectId Id { get; set; }

    /// <summary>
    /// The order in the queue.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// The custom message of the queue item.
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// The number of times the item has gotten dequeued.
    /// </summary>
    public int DequeueCount { get; set; }

    /// <summary>
    /// The UTC times at which the item has been dequeued.
    /// </summary>
    public IList<DateTime> DequeuedWhen { get; set; } = null!;

    /// <summary>
    /// At which time the item can be dequeued.
    /// </summary>
    public DateTime VisibleWhen { get; set; }

    /// <summary>
    /// When the item was added to the queue.
    /// </summary>
    public DateTime CreatedWhen { get; set; }

    /// <summary>
    /// When the item got acknowledged.
    /// </summary>
    public DateTime? AcknowledgedWhen { get; set; }
}
