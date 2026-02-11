using Acceloka.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Features.RevokeTicket
{
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketCommand, RevokeTicketResponse>
    {
        private readonly AccelokaDbContext _context;

        public RevokeTicketHandler(AccelokaDbContext context)
        {
            _context = context;
        }

        public async Task<RevokeTicketResponse> Handle(RevokeTicketCommand request, CancellationToken cancellationToken)
        {
            // Check if booked ticket exists
            var bookedTicket = await _context.BookedTickets
                .FirstOrDefaultAsync(bt => bt.Id == request.BookedTicketId, cancellationToken);

            if (bookedTicket == null)
            {
                throw new InvalidOperationException($"Booked ticket with ID {request.BookedTicketId} not found");
            }

            // Check if booked ticket detail exists
            var bookedDetail = await _context.BookedTicketDetails
                .FirstOrDefaultAsync(btd =>
                    btd.BookedTicketId == request.BookedTicketId &&
                    btd.TicketCode == request.TicketCode,
                    cancellationToken);

            if (bookedDetail == null)
            {
                throw new InvalidOperationException($"Ticket {request.TicketCode} not found in booking {request.BookedTicketId}");
            }

            // Validation
            if (request.Quantity > bookedDetail.Quantity)
            {
                throw new InvalidOperationException($"Cannot revoke {request.Quantity} tickets. Only {bookedDetail.Quantity} were booked.");
            }

            // Get ticket to restore quota
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Code == request.TicketCode, cancellationToken);

            if (ticket == null)
            {
                throw new InvalidOperationException($"Ticket {request.TicketCode} not found");
            }

            ticket.Quota += request.Quantity;

            // Update or delete booked ticket detail
            if (request.Quantity == bookedDetail.Quantity)
            {
                // Remove entire detail
                _context.BookedTicketDetails.Remove(bookedDetail);
            }
            else
            {
                // Reduce quantity
                bookedDetail.Quantity -= request.Quantity;
            }

            // Check if booking has any remaining details
            var remainingDetails = await _context.BookedTicketDetails
                .Where(btd => btd.BookedTicketId == request.BookedTicketId && btd.Id != bookedDetail.Id)
                .AnyAsync(cancellationToken);

            // If no remaining details and current detail is being removed, then delete the booking
            if (!remainingDetails && request.Quantity == bookedDetail.Quantity)
            {
                _context.BookedTickets.Remove(bookedTicket);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new RevokeTicketResponse
            {
                Message = $"Successfully revoked {request.Quantity} ticket(s) of {request.TicketCode}",
                RestoredQuota = request.Quantity
            };
        }
    }
}