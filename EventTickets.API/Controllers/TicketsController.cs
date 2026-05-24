using EventTickets.API.Models.Requests;
using EventTickets.API.Models.Responses;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EventTickets.API.Controllers
{
    [ApiController]
    [Route("api/v1/events/{eventId}/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly ITimeProvider _timeProvider;

        public TicketsController(
            ITicketRepository ticketRepo,
            IUnitOfWorkRepository unitOfWork,
            ITimeProvider timeProvider)
        {
            _ticketRepo = ticketRepo;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveTicket(
            int eventId,
            [FromBody] ReserveTicketRequest request,
            CancellationToken cancellationToken)
        {
            var now = _timeProvider.UtcNow;

            var ticket = await _ticketRepo.TryReserveAvailableTicketAsync(
                eventId,
                request.HolderName,
                now,
                cancellationToken);

            if (ticket is null)
                throw new ConflictException("No available tickets for this event.");

            return Ok(TicketResponse.FromEntity(ticket));
        }

        [HttpPost("{ticketId}/purchase")]
        public async Task<IActionResult> PurchaseTicket(
            int eventId,
            int ticketId,
            [FromBody] PurchaseTicketRequest request,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepo.GetTicketByIdAsync(ticketId, cancellationToken);

            if (ticket is null || ticket.EventId != eventId)
                throw new NotFoundException("Ticket not found.");

            if (ticket.Status != Domain.Enums.TicketStatus.Reserved)
                throw new ConflictException("Ticket is not reserved.");

            if (ticket.HolderName != request.HolderName)
                throw new ConflictException("Ticket holder name mismatch.");

            // Mark as sold
            ticket.Status = Domain.Enums.TicketStatus.Sold;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(TicketResponse.FromEntity(ticket));
        }
    }

}
