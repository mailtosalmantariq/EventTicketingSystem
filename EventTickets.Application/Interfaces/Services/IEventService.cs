using EventTickets.Application.DTOs;

namespace EventTickets.Application.Interfaces.Services
{
    public interface IEventService
    {
        Task<EventDetailsDto> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken);
    }
}
