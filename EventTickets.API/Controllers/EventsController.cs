using EventTickets.API.Models.Responses;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventTickets.API.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(int eventId, CancellationToken cancellationToken)
        {
            var result = await _eventService.GetEventDetailsAsync(eventId, cancellationToken);
            return Ok(result);
        }
    }


}
