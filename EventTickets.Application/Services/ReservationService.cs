using EventTickets.Application.DTOs;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Application.Interfaces.Services;

namespace EventTickets.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ITicketRepository _tickets;
        private readonly IUnitOfWorkRepository _uow;
        private readonly ITimeProvider _clock;

        public ReservationService(
            ITicketRepository tickets,
            IUnitOfWorkRepository uow,
            ITimeProvider clock)
        {
            _tickets = tickets;
            _uow = uow;
            _clock = clock;
        }

        public async Task<int> ReserveTicketAsync(int eventId, ReserveTicketRequest request, CancellationToken cancellationToken)
        {
            var now = _clock.UtcNow;

            var ticket = await _tickets.TryReserveAvailableTicketAsync(
                eventId,
                request.HolderName,
                now,
                cancellationToken);

            if (ticket is null)
                throw new ConflictException("No available tickets remain for this event.");

            await _uow.SaveChangesAsync(cancellationToken);

            return ticket.Id;
        }
    }

}
