using EventTickets.Domain.Entities;

namespace EventTickets.Application.Interfaces.Repos
{
    public interface IEventRepository
    {
        Task<Event?> GetEventWithTicketsAsync(int eventId, CancellationToken cancellationToken);
    }
}
