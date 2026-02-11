using Acceloka.API.Data;
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
            var bookedTicket = new BookedTicket
            {
                BookedDate = DateTime.UtcNow
            };

            _context.BookedTickets.Add(bookedTicket);
            await _context.SaveChangesAsync(cancellationToken);

            var bookedTicketDetails = new List<BookedTicketDetail>();
            decimal totalPrice = 0;

            var codes = request.Tickets.Select(t => t.KodeTicket).ToList();

            var ticketsWithCategories = await _context.Tickets
                .Include(t => t.Category)
                .Where(t => codes.Contains(t.Code))
                .ToListAsync(cancellationToken);

            foreach (var item in request.Tickets)
            {
                var ticket = ticketsWithCategories.FirstOrDefault(t => t.Code == item.KodeTicket);

                if (ticket == null)
                {
                    throw new InvalidOperationException($"Ticket {item.KodeTicket} not found");
                }

                
                var bookedDetail = new BookedTicketDetail
                {
                    BookedTicketId = bookedTicket.Id,
                    TicketCode = item.KodeTicket,
                    Quantity = item.Qty
                };

                _context.BookedTicketDetails.Add(bookedDetail);
                bookedTicketDetails.Add(bookedDetail);

                ticket.Quota -= item.Qty;
                totalPrice += ticket.Price * item.Qty;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var categoryGroups = bookedTicketDetails
                .GroupBy(btd => ticketsWithCategories.First(t => t.Code == btd.TicketCode).Category.Name)
                .Select(g => new CategoryBookingDto
                {
                    CategoryName = g.Key,
                    Tickets = g.Select(btd =>
                    {
                        var ticket = ticketsWithCategories.First(t => t.Code == btd.TicketCode);
                        return new BookedItemDto
                        {
                            KodeTicket = btd.TicketCode,
                            NamaTicket = ticket.Name,
                            Qty = btd.Quantity,
                            Price = ticket.Price,
                            SubTotal = ticket.Price * btd.Quantity
                        };
                    }).ToList(),
                    CategoryTotal = g.Sum(btd =>
                    {
                        var ticket = ticketsWithCategories.First(t => t.Code == btd.TicketCode);
                        return ticket.Price * btd.Quantity;
                    })
                })
                .ToList();

            return new BookTicketResponse
            {
                BookedTicketId = bookedTicket.Id,
                BookedDate = bookedTicket.BookedDate,
                Categories = categoryGroups,
                GrandTotal = totalPrice
            };
        }
    }
}