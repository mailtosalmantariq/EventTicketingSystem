
namespace EventTickets.Application.Interfaces.Services
{
    public interface IPurchaseService
    {
        Task PurchaseAsync(int ticketId, string holderName, CancellationToken cancellationToken);
    }
}
