using EventTickets.Application.Interfaces.Repos;
using EventTickets.Data.DataContext;
using EventTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTickets.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _db;

        public EventRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Event?> GetEventWithTicketsAsync(int eventId, CancellationToken cancellationToken)
        {
            return await _db.Events
            .AsNoTracking()
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        }


    }

}
