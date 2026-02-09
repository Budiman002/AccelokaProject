namespace Acceloka.API.Models
{
    public class BookedTicket
    {
        public int Id { get; set; }
        public DateTime BookedDate { get; set; }

        public ICollection<BookedTicketDetail> BookedTicketDetails { get; set; } = new List<BookedTicketDetail>();
    }
}