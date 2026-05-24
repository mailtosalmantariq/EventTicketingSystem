using EventTickets.Application.Interfaces.Repos;
using EventTickets.Data.DataContext;
using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTickets.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _db;

        public TicketRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Ticket?> TryReserveAvailableTicketAsync(
           int eventId,
           string holderName,
           DateTime nowUtc,
           CancellationToken cancellationToken)
        {
            // Load with tracking so EF can use RowVersion
            var ticket = await _db.Tickets
                .Where(t => t.EventId == eventId &&
                            t.Status == TicketStatus.Available)
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (ticket is null)
                return null;

            ticket.Status = TicketStatus.Reserved;
            ticket.HolderName = holderName;
            ticket.ReservedAt = nowUtc;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Someone else reserved it first
                return null;
            }

            return ticket;
        }


        public async Task<Ticket?> GetTicketByIdAsync(
            int ticketId,
            CancellationToken cancellationToken)
        {
            return await _db.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken);
        }

        public async Task UpdateAsync(
            Ticket ticket,
            CancellationToken cancellationToken)
        {
            _db.Tickets.Update(ticket);
            await Task.CompletedTask;
        }
    }
}
