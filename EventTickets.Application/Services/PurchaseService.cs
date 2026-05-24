using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Application.Interfaces.Services;
using EventTickets.Domain.Enums;

namespace EventTickets.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ITicketRepository _tickets;
        private readonly IUnitOfWorkRepository _uow;
        private readonly ITimeProvider _clock;

        public PurchaseService(
            ITicketRepository tickets,
            IUnitOfWorkRepository uow,
            ITimeProvider clock)
        {
            _tickets = tickets;
            _uow = uow;
            _clock = clock;
        }

        public async Task PurchaseAsync(int ticketId, string holderName, CancellationToken cancellationToken)
        {
            var ticket = await _tickets.GetTicketByIdAsync(ticketId, cancellationToken)
                         ?? throw new NotFoundException($"Ticket {ticketId} not found.");

            var now = _clock.UtcNow;

            var isExpired = ticket.Status == TicketStatus.Reserved &&
                            ticket.ReservedAt < now.AddMinutes(-10);

            if (ticket.Status != TicketStatus.Reserved || isExpired)
                throw new ConflictException("Ticket is not reserved or reservation expired.");

            if (!string.Equals(ticket.HolderName, holderName, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Ticket is reserved by a different holder.");

            ticket.Status = TicketStatus.Sold;
            ticket.ReservedAt = null;

            await _tickets.UpdateAsync(ticket, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

}
