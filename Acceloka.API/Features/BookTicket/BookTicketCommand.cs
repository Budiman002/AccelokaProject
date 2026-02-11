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
        public List<CategoryBookingDto> Categories { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }

    public class CategoryBookingDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<BookedItemDto> Tickets { get; set; } = new();
        public decimal CategoryTotal { get; set; }
    }

    public class BookedItemDto
    {
        public string KodeTicket { get; set; } = string.Empty;
        public string NamaTicket { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
    }
}