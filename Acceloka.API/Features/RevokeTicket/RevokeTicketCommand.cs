using MediatR;

namespace Acceloka.API.Features.RevokeTicket
{
    public class RevokeTicketCommand : IRequest<RevokeTicketResponse>
    {
        public int BookedTicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class RevokeTicketResponse
    {
        public string Message { get; set; } = string.Empty;
        public int RestoredQuota { get; set; }
    }
}