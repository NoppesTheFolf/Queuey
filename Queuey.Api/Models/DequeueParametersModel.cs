namespace Noppes.Queuey.Api.Models;

public class DequeueParametersModel
{
    public int? Limit { get; set; }

    public TimeSpan? VisibilityDelay { get; set; }
}
