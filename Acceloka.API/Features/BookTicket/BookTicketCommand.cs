using Acceloka.API.Features.BookTicket;
using Acceloka.API.Models;
using MediatR;

namespace Acceloka.API.Features.BookTicket
{
    public class BookTicketCommand : IRequest<BookTicketResponse>
    {
        public List<BookTicketItemDto> Tickets { get; set; } = new();
    }

    public class BookTicketItemDto
    {
        public string KodeTicket { get; set; } = string.Empty;
        public int Qty { get; set; }
    }

    public class BookTicketResponse
    {
        public int BookedTicketId { get; set; }
        public DateTime BookedDate { get; set; }
        public List<BookedItemDto> BookedTickets { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class BookedItemDto
    {
        public string KodeTicket { get; set; } = string.Empty;
        public string NamaTicket { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal PricePerTicket { get; set; }
        public decimal SubTotal { get; set; }
    }
}