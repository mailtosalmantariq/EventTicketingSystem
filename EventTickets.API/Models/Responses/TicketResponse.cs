using EventTickets.Domain.Entities;

namespace EventTickets.API.Models.Responses
{
    public class TicketResponse
    {
        public int Id { get; set; }
        public string? HolderName { get; set; }
        public string Status { get; set; } = string.Empty;

        public static TicketResponse FromEntity(Ticket ticket)
        {
            return new TicketResponse
            {
                Id = ticket.Id,
                HolderName = ticket.HolderName,
                Status = ticket.Status.ToString()
            };
        }
    }

}
