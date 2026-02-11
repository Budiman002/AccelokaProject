using MediatR;

namespace Acceloka.API.Features.EditBookedTicket
{
    public class EditBookedTicketCommand : IRequest<EditBookedTicketResponse>
    {
        public int BookedTicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int NewQuantity { get; set; }
    }

    public class EditBookedTicketResponse
    {
        public string Message { get; set; } = string.Empty;
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public int QuotaAdjustment { get; set; }
    }
}