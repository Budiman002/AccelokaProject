using Acceloka.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;

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
            var bookedTicket = await _context.BookedTickets
                .FirstOrDefaultAsync(bt => bt.Id == request.BookedTicketId, cancellationToken);

            if (bookedTicket == null)
            {
                throw new InvalidOperationException($"Booked ticket with ID {request.BookedTicketId} not found");
            }

            var bookedDetail = await _context.BookedTicketDetails
                .FirstOrDefaultAsync(btd =>
                    btd.BookedTicketId == request.BookedTicketId &&
                    btd.TicketCode == request.TicketCode,
                    cancellationToken);

            if (bookedDetail == null)
            {
                throw new InvalidOperationException($"Ticket {request.TicketCode} not found in booking {request.BookedTicketId}");
            }

            if (request.Quantity > bookedDetail.Quantity)
            {
                throw new InvalidOperationException($"Cannot revoke {request.Quantity} tickets. Only {bookedDetail.Quantity} were booked.");
            }

            var ticket = await _context.Tickets
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Code == request.TicketCode, cancellationToken);

            if (ticket == null)
            {
                throw new InvalidOperationException($"Ticket {request.TicketCode} not found");
            }

            ticket.Quota += request.Quantity;

            int sisaQuantity = bookedDetail.Quantity - request.Quantity;

            if (request.Quantity == bookedDetail.Quantity)
            {
                _context.BookedTicketDetails.Remove(bookedDetail);
            }
            else
            {
                bookedDetail.Quantity -= request.Quantity;
            }

            var remainingDetails = await _context.BookedTicketDetails
                .Where(btd => btd.BookedTicketId == request.BookedTicketId && btd.Id != bookedDetail.Id)
                .AnyAsync(cancellationToken);

            if (!remainingDetails && request.Quantity == bookedDetail.Quantity)
            {
                _context.BookedTickets.Remove(bookedTicket);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new RevokeTicketResponse
            {
                KodeTicket = request.TicketCode,
                NamaTicket = ticket.Name,
                NamaKategori = ticket.Category.Name,
                SisaQuantity = sisaQuantity
            };
        }
    }
}