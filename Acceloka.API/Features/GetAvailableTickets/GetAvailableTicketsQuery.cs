using MediatR;

namespace Acceloka.API.Features.GetAvailableTickets
{
    public class GetAvailableTicketsQuery : IRequest<GetAvailableTicketsResponse>
    {
        public string? NamaKategori { get; set; }
        public string? KodeTicket { get; set; }
        public string? NamaTicket { get; set; }
        public decimal? Harga { get; set; }
        public DateTime? TanggalEventMin { get; set; }
        public DateTime? TanggalEventMax { get; set; }

        public string? Search { get; set; }

        public string? OrderBy { get; set; }
        public string? OrderState { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetAvailableTicketsResponse
    {
        public List<TicketDto> Tickets { get; set; } = new();
        public int TotalCount { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
    }

    public class TicketDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
        public int Quota { get; set; }
    }
}