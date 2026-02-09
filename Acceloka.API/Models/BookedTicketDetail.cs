using Acceloka.API.Models;

namespace Acceloka.API.Models
{
    public class BookedTicketDetail
    {
        public int Id { get; set; }
        public int BookedTicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public BookedTicket BookedTicket { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;
    }
}