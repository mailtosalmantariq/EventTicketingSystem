using EventTickets.Domain.Entities;

namespace EventTickets.Application.Interfaces.Repos
{
    public interface ITicketRepository
    {
        Task<Ticket?> TryReserveAvailableTicketAsync(
            int eventId,
            string holderName,
            DateTime reservedAt,
            CancellationToken cancellationToken);

        Task<Ticket?> GetTicketByIdAsync(
            int ticketId,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            Ticket ticket,
            CancellationToken cancellationToken);
    }
}
