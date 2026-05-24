
namespace EventTickets.Application.DTOs
{
    public class EventDetailsDto
    {
        public int Id { get; }
        public string Name { get; }
        public DateTime StartsAt { get; }
        public int Available { get; }
        public int Reserved { get; }
        public int Sold { get; }

        public EventDetailsDto(
            int id,
            string name,
            DateTime startsAt,
            int available,
            int reserved,
            int sold)
        {
            Id = id;
            Name = name;
            StartsAt = startsAt;
            Available = available;
            Reserved = reserved;
            Sold = sold;
        }
    }
}
