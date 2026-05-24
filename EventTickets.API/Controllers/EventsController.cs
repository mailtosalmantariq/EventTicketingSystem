using EventTickets.API.Models.Responses;
using EventTickets.Application.Interfaces.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventTickets.API.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepo;

        public EventsController(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(int eventId, CancellationToken cancellationToken)
        {
            var ev = await _eventRepo.GetEventWithTicketsAsync(eventId, cancellationToken);

            if (ev is null)
                return NotFound();

            return Ok(EventResponse.FromEntity(ev));
        }
    }

}
