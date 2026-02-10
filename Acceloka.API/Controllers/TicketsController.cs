using Acceloka.API.Features.GetAvailableTickets;
using Acceloka.API.Features.BookTicket;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Acceloka.API.Features.GetBookedTicketDetail;

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

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTicketDetail([FromRoute] int bookedTicketId)
        {
            var query = new GetBookedTicketDetailQuery
            {
                BookedTicketId = bookedTicketId
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}