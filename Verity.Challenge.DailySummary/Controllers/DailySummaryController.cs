using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Verity.Challenge.DailySummary.Application.DailySummary.Handlers.GetDailySummary;

namespace Verity.Challenge.DailySummary.Controllers;

[Route("api/daily-summary")]
[ApiController]
public class DailySummaryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime date)
    {
        var summary = await mediator.Send(new GetDailySummaryQuery(date));

        return summary is null ? NotFound() : Ok(summary);
    }
}