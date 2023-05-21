namespace Noppes.Queuey.Api.Models;

public class EnqueueItemModel
{
    public int? Priority { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? VisibleWhen { get; set; }
}
