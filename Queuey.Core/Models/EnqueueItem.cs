namespace Noppes.Queuey.Core.Models;

public class EnqueueItem
{
    public int Priority { get; set; }

    public string Message { get; set; } = null!;

    public DateTime VisibleWhen { get; set; }
}
