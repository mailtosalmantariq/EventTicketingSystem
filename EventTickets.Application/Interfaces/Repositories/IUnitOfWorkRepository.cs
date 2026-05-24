namespace EventTickets.Application.Interfaces.Repositories
{
    public interface IUnitOfWorkRepository
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
