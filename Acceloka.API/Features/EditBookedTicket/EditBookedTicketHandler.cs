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
            var bookedTicket = await _context.BookedTickets
                .FirstOrDefaultAsync(bt => bt.Id == request.BookedTicketId, cancellationToken);

            if (bookedTicket == null)
            {
                throw new InvalidOperationException($"Booked ticket with ID {request.BookedTicketId} not found");
            }

            var editedTickets = new List<EditedTicketDto>();

            foreach (var item in request.Tickets)
            {

                var bookedDetail = await _context.BookedTicketDetails
                    .FirstOrDefaultAsync(btd =>
                        btd.BookedTicketId == request.BookedTicketId &&
                        btd.TicketCode == item.TicketCode,
                        cancellationToken);

                if (bookedDetail == null)
                {
                    throw new InvalidOperationException($"Ticket {item.TicketCode} not found in booking {request.BookedTicketId}");
                }

                var ticket = await _context.Tickets
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Code == item.TicketCode, cancellationToken);

                if (ticket == null)
                {
                    throw new InvalidOperationException($"Ticket {item.TicketCode} not found");
                }

                int oldQuantity = bookedDetail.Quantity;
                int quantityDifference = item.NewQuantity - oldQuantity;


                if (quantityDifference > 0)
                {
                    if (ticket.Quota < quantityDifference)
                    {
                        throw new InvalidOperationException($"Insufficient quota for {item.TicketCode}. Available: {ticket.Quota}, Requested increase: {quantityDifference}");
                    }
                }

                ticket.Quota -= quantityDifference;


                bookedDetail.Quantity = item.NewQuantity;


                editedTickets.Add(new EditedTicketDto
                {
                    KodeTicket = item.TicketCode,
                    NamaTicket = ticket.Name,
                    NamaKategori = ticket.Category.Name,
                    SisaQuantity = item.NewQuantity
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new EditBookedTicketResponse
            {
                EditedTickets = editedTickets
            };
        }
    }
}