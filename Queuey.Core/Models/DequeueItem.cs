namespace Noppes.Queuey.Core.Models;

public class DequeueItem
{
    public string Id { get; set; } = null!;

    public int Priority { get; set; }

    public int DequeueCount { get; set; }

    public string Message { get; set; } = null!;
}