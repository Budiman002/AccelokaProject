using Acceloka.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Features.GetBookedTicketDetail
{
    public class GetBookedTicketDetailHandler : IRequestHandler<GetBookedTicketDetailQuery, GetBookedTicketDetailResponse>
    {
        private readonly AccelokaDbContext _context;

        public GetBookedTicketDetailHandler(AccelokaDbContext context)
        {
            _context = context;
        }

        public async Task<GetBookedTicketDetailResponse> Handle(GetBookedTicketDetailQuery request, CancellationToken cancellationToken)
        {
            // Get booked ticket
            var bookedTicket = await _context.BookedTickets
                .FirstOrDefaultAsync(bt => bt.Id == request.BookedTicketId, cancellationToken);

            if (bookedTicket == null)
            {
                throw new InvalidOperationException($"Booked ticket with ID {request.BookedTicketId} not found");
            }

            // Get booked ticket details with related data
            var details = await _context.BookedTicketDetails
                .Include(btd => btd.Ticket)
                    .ThenInclude(t => t.Category)
                .Where(btd => btd.BookedTicketId == request.BookedTicketId)
                .ToListAsync(cancellationToken);

            // Group by category
            var categoryGroups = details
                .GroupBy(d => new { d.Ticket.Category.Id, d.Ticket.Category.Name })
                .Select(g => new CategoryGroupDto
                {
                    CategoryName = g.Key.Name,
                    Tickets = g.Select(d => new TicketDetailDto
                    {
                        KodeTicket = d.TicketCode,
                        NamaTicket = d.Ticket.Name,
                        Qty = d.Quantity,
                        Price = d.Ticket.Price,
                        SubTotal = d.Ticket.Price * d.Quantity
                    }).ToList(),
                    CategoryTotal = g.Sum(d => d.Ticket.Price * d.Quantity)
                })
                .ToList();

            var grandTotal = categoryGroups.Sum(c => c.CategoryTotal);

            return new GetBookedTicketDetailResponse
            {
                BookedTicketId = bookedTicket.Id,
                BookedDate = bookedTicket.BookedDate,
                Categories = categoryGroups,
                GrandTotal = grandTotal
            };
        }
    }
}