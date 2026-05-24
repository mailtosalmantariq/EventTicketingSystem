

using EventTickets.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventTickets.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        [JsonIgnore]
        public Event Event { get; set; } = null!;

        public string? HolderName { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Available;

        public DateTime? ReservedAt { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

    }
}
