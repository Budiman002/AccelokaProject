namespace Acceloka.API.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
        public int Quota { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<BookedTicketDetail> BookedTicketDetails { get; set; } = new List<BookedTicketDetail>();
    }
}