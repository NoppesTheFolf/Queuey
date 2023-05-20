namespace Noppes.Queuey.Api.Models;

public class DequeueItemModel
{
    public string Id { get; set; } = null!;

    public string Message { get; set; } = null!;
}