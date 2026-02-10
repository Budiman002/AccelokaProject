using Acceloka.API.Data;
using Acceloka.API.Features.BookTicket;
using Acceloka.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Features.BookTicket
{
    public class BookTicketHandler : IRequestHandler<BookTicketCommand, BookTicketResponse>
    {
        private readonly AccelokaDbContext _context;

        public BookTicketHandler(AccelokaDbContext context)
        {
            _context = context;
        }

        public async Task<BookTicketResponse> Handle(BookTicketCommand request, CancellationToken cancellationToken)
        {
            // Create BookedTicket (master record)
            var bookedTicket = new BookedTicket
            {
                BookedDate = DateTime.UtcNow
            };

            _context.BookedTickets.Add(bookedTicket);
            await _context.SaveChangesAsync(cancellationToken);

            var bookedItems = new List<BookedItemDto>();
            decimal totalPrice = 0;

            // Process each ticket
            foreach (var item in request.Tickets)
            {
                // Get ticket details
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.Code == item.KodeTicket, cancellationToken);

                if (ticket == null)
                {
                    throw new InvalidOperationException($"Ticket {item.KodeTicket} not found");
                }

                // Create BookedTicketDetail
                var bookedDetail = new BookedTicketDetail
                {
                    BookedTicketId = bookedTicket.Id,
                    TicketCode = item.KodeTicket,
                    Quantity = item.Qty
                };

                _context.BookedTicketDetails.Add(bookedDetail);

                // Update ticket quota
                ticket.Quota -= item.Qty;

                // Calculate prices
                var subTotal = ticket.Price * item.Qty;
                totalPrice += subTotal;

                bookedItems.Add(new BookedItemDto
                {
                    KodeTicket = ticket.Code,
                    NamaTicket = ticket.Name,
                    Qty = item.Qty,
                    PricePerTicket = ticket.Price,
                    SubTotal = subTotal
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new BookTicketResponse
            {
                BookedTicketId = bookedTicket.Id,
                BookedDate = bookedTicket.BookedDate,
                BookedTickets = bookedItems,
                TotalPrice = totalPrice
            };
        }
    }
}