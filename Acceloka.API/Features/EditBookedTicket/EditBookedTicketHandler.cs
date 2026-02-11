using Acceloka.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.API.Features.EditBookedTicket
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketCommand, EditBookedTicketResponse>
    {
        private readonly AccelokaDbContext _context;

        public EditBookedTicketHandler(AccelokaDbContext context)
        {
            _context = context;
        }

        public async Task<EditBookedTicketResponse> Handle(EditBookedTicketCommand request, CancellationToken cancellationToken)
        {
            // Validate booked ticket exists
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

            // Get ticket to adjust quota
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Code == request.TicketCode, cancellationToken);

            if (ticket == null)
            {
                throw new InvalidOperationException($"Ticket {request.TicketCode} not found");
            }

            int oldQuantity = bookedDetail.Quantity;
            int quantityDifference = request.NewQuantity - oldQuantity;

            if (quantityDifference > 0)
            {
                if (ticket.Quota < quantityDifference)
                {
                    throw new InvalidOperationException($"Insufficient quota. Available: {ticket.Quota}, Requested increase: {quantityDifference}");
                }
            }
            ticket.Quota -= quantityDifference; 

            // Update booked detail quantity
            bookedDetail.Quantity = request.NewQuantity;

            await _context.SaveChangesAsync(cancellationToken);

            return new EditBookedTicketResponse
            {
                Message = $"Successfully updated {request.TicketCode} quantity from {oldQuantity} to {request.NewQuantity}",
                OldQuantity = oldQuantity,
                NewQuantity = request.NewQuantity,
                QuotaAdjustment = -quantityDifference 
            };
        }
    }
}