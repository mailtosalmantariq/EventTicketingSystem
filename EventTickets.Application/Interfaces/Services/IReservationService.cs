using EventTickets.Application.DTOs;

namespace EventTickets.Application.Interfaces.Services
{
    public interface IReservationService
    {
        Task<int> ReserveTicketAsync(int eventId, ReserveTicketRequestDto request, CancellationToken cancellationToken);
    }
}
