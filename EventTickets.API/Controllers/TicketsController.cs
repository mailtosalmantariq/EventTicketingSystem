using EventTickets.API.Models.Requests;
using EventTickets.API.Models.Responses;
using EventTickets.Application.DTOs;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventTickets.API.Controllers
{
    [ApiController]
    [Route("api/v1/events/{eventId}/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IPurchaseService _purchaseService;

        public TicketsController(
            IReservationService reservationService,
            IPurchaseService purchaseService)
        {
            _reservationService = reservationService;
            _purchaseService = purchaseService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveTicket(
            int eventId,
            [FromBody] ReserveTicketRequest request,
            CancellationToken cancellationToken)
        {

            var dto = new ReserveTicketRequestDto
            {
                HolderName = request.HolderName
            };

            var ticketId = await _reservationService.ReserveTicketAsync(
                eventId,
                dto,
                cancellationToken);


            return Ok(new { TicketId = ticketId });
        }

        [HttpPost("{ticketId}/purchase")]
        public async Task<IActionResult> PurchaseTicket(
            int eventId,
            int ticketId,
            [FromBody] PurchaseTicketRequest request,
            CancellationToken cancellationToken)
        {
            await _purchaseService.PurchaseAsync(
                ticketId,
                request.HolderName,
                cancellationToken);

            return Ok(new { TicketId = ticketId, Status = "Sold" });
        }
    }


}
