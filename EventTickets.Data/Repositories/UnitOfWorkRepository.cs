using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Data.DataContext;

namespace EventTickets.Data.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly AppDbContext _db;

        public UnitOfWorkRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _db.SaveChangesAsync(cancellationToken);
    }

}
