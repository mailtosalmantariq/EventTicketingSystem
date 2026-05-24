
namespace EventTickets.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; }
        public string? HolderName { get; }
        public string Status { get; }
        public DateTime? ReservedAt { get; }

        public TicketDto(int id, string? holderName, string status, DateTime? reservedAt)
        {
            Id = id;
            HolderName = holderName;
            Status = status;
            ReservedAt = reservedAt;
        }
    }
}
