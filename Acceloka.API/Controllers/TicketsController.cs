using Acceloka.API.Features.GetAvailableTickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Acceloka.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets(
            [FromQuery] string? search,
            [FromQuery] string? orderBy,
            [FromQuery] string? orderState,
            [FromQuery] int? page,
            [FromQuery] int? pageSize)
        {
            var query = new GetAvailableTicketsQuery
            {
                Search = search,
                OrderBy = orderBy,
                OrderState = orderState,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}