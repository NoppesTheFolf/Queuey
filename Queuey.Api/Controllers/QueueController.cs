using Microsoft.AspNetCore.Mvc;
using Noppes.Queuey.Api.Models;
using Noppes.Queuey.Core;
using Noppes.Queuey.Core.Models;

namespace Noppes.Queuey.Api.Controllers;

[ApiController]
[Route("queue")]
public class QueueController : ControllerBase
{
    private readonly IQueueProvider _queueProvider;

    public QueueController(IQueueProvider queueProvider)
    {
        _queueProvider = queueProvider;
    }

    [HttpPost]
    [Route("{name}/enqueue")]
    public async Task<IActionResult> Enqueue(string name, [FromBody] ICollection<EnqueueItemModel> models)
    {
        var items = models.Select(x => new EnqueueItem
        {
            Message = x.Message,
            VisibleWhen = x.VisibleWhen ?? DateTime.UtcNow,
            Priority = x.Priority ?? int.MinValue
        }).ToList();
        var queue = _queueProvider.Get(name);
        var ids = await queue.EnqueueAsync(items);

        return Ok(ids);
    }

    [HttpGet]
    [Route("{name}/dequeue")]
    public async Task<IActionResult> Dequeue(string name, int? limit, TimeSpan? visibilityDelay)
    {
        var queue = _queueProvider.Get(name);
        var items = await queue.DequeueAsync(limit ?? 1, visibilityDelay);

        var models = items.Select(x => new DequeueItemModel
        {
            Id = x.Id,
            Message = x.Message
        }).ToList();
        return Ok(models);
    }

    [HttpGet]
    [Route("{name}/{id}/acknowledge")]
    public async Task<IActionResult> Acknowledge(string name, string id)
    {
        var queue = _queueProvider.Get(name);
        var couldBeFound = await queue.AcknowledgeAsync(id);

        return couldBeFound ? Ok() : NotFound();
    }
}
