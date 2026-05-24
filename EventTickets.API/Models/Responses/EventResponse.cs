using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;

namespace EventTickets.API.Models.Responses
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableTickets { get; set; }
        public int ReservedTickets { get; set; }
        public int SoldTickets { get; set; }

        public static EventResponse FromEntity(Event ev)
        {
            var tickets = ev.Tickets.ToList(); 

            return new EventResponse
            {
                Id = ev.Id,
                Name = ev.Name,
                StartsAt = ev.StartsAt,
                TotalSeats = ev.TotalSeats,
                AvailableTickets = tickets.Count(t => t.Status == TicketStatus.Available),
                ReservedTickets = tickets.Count(t => t.Status == TicketStatus.Reserved),
                SoldTickets = tickets.Count(t => t.Status == TicketStatus.Sold)
            };

        }
    }

}
