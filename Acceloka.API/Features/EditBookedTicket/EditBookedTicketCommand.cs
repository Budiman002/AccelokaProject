using MediatR;

namespace Acceloka.API.Features.EditBookedTicket
{
    public class EditBookedTicketCommand : IRequest<EditBookedTicketResponse>
    {
        public int BookedTicketId { get; set; }
        public List<EditTicketItemDto> Tickets { get; set; } = new();
    }

    public class EditTicketItemDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public int NewQuantity { get; set; }
    }


    public class EditBookedTicketResponse
    {
        public List<EditedTicketDto> EditedTickets { get; set; } = new();
    }

    public class EditedTicketDto
    {
        public string KodeTicket { get; set; } = string.Empty;
        public string NamaTicket { get; set; } = string.Empty;
        public string NamaKategori { get; set; } = string.Empty;
        public int SisaQuantity { get; set; }
    }
}