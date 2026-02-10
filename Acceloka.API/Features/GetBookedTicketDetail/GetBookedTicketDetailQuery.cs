using MediatR;

namespace Acceloka.API.Features.GetBookedTicketDetail
{
    public class GetBookedTicketDetailQuery : IRequest<GetBookedTicketDetailResponse>
    {
        public int BookedTicketId { get; set; }
    }

    public class GetBookedTicketDetailResponse
    {
        public int BookedTicketId { get; set; }
        public DateTime BookedDate { get; set; }
        public List<CategoryGroupDto> Categories { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }

    public class CategoryGroupDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<TicketDetailDto> Tickets { get; set; } = new();
        public decimal CategoryTotal { get; set; }
    }

    public class TicketDetailDto
    {
        public string KodeTicket { get; set; } = string.Empty;
        public string NamaTicket { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
    }
}