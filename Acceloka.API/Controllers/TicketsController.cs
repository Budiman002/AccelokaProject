using Acceloka.API.Features.GetAvailableTickets;
using Acceloka.API.Features.BookTicket;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Acceloka.API.Features.GetBookedTicketDetail;
using Acceloka.API.Features.RevokeTicket;
using Acceloka.API.Features.EditBookedTicket;

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
            [FromQuery] string? namaKategori,
            [FromQuery] string? kodeTicket,
            [FromQuery] string? namaTicket,
            [FromQuery] decimal? harga,
            [FromQuery] DateTime? tanggalEventMin,
            [FromQuery] DateTime? tanggalEventMax,
            [FromQuery] string? search,
            [FromQuery] string? orderBy,
            [FromQuery] string? orderState,
            [FromQuery] int? page,
            [FromQuery] int? pageSize)
        {
            var query = new GetAvailableTicketsQuery
            {
                NamaKategori = namaKategori,
                KodeTicket = kodeTicket,
                NamaTicket = namaTicket,
                Harga = harga,
                TanggalEventMin = tanggalEventMin,
                TanggalEventMax = tanggalEventMax,
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

        [HttpDelete("revoke-ticket/{bookedTicketId}/{ticketCode}/{quantity}")]
        public async Task<IActionResult> RevokeTicket(
            [FromRoute] int bookedTicketId,
            [FromRoute] string ticketCode,
            [FromRoute] int quantity)
        {
            var command = new RevokeTicketCommand
            {
                BookedTicketId = bookedTicketId,
                TicketCode = ticketCode,
                Quantity = quantity
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(
            [FromRoute] int bookedTicketId,
            [FromBody] EditBookedTicketRequest request)
        {
            var command = new EditBookedTicketCommand
            {
                BookedTicketId = bookedTicketId,
                Tickets = request.Tickets
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
        public class EditBookedTicketRequest
        {
            public List<EditTicketItemDto> Tickets { get; set; } = new();
        }
    
}