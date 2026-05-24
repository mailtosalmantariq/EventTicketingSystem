using EventTickets.Application.DTOs;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Services;
using EventTickets.Domain.Enums;

namespace EventTickets.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _events;
        private readonly ITimeProvider _clock;

        public EventService(IEventRepository events, ITimeProvider clock)
        {
            _events = events;
            _clock = clock;
        }

        public async Task<EventDetailsDto> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken)
        {
            var ev = await _events.GetEventWithTicketsAsync(eventId, cancellationToken)
                     ?? throw new NotFoundException($"Event {eventId} not found.");

            var now = _clock.UtcNow;

            var available = ev.Tickets.Count(t =>
                t.Status == TicketStatus.Available ||
                (t.Status == TicketStatus.Reserved && t.ReservedAt < now.AddMinutes(-10)));

            var reserved = ev.Tickets.Count(t =>
                t.Status == TicketStatus.Reserved && t.ReservedAt >= now.AddMinutes(-10));

            var sold = ev.Tickets.Count(t => t.Status == TicketStatus.Sold);

            return new EventDetailsDto(ev.Id, ev.Name, ev.StartsAt, available, reserved, sold);
        }
    }

}
