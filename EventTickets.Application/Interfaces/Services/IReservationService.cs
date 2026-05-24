using EventTickets.Application.DTOs;

namespace EventTickets.Application.Interfaces.Services
{
    public interface IReservationService
    {
        Task<int> ReserveTicketAsync(int eventId, ReserveTicketRequest request, CancellationToken cancellationToken);
    }
}
