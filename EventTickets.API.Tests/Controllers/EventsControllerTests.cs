using EventTickets.API.Controllers;
using EventTickets.API.Models.Responses;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventTickets.API.Tests.Controllers
{
    [TestFixture]
    public class EventsControllerTests
    {
        private Mock<IEventRepository> _eventRepoMock;
        private EventsController _controller;

        [SetUp]
        public void SetUp()
        {
            _eventRepoMock = new Mock<IEventRepository>();
            _controller = new EventsController(_eventRepoMock.Object);
        }

        private Event CreateEvent(int id)
        {
            return new Event
            {
                Id = id,
                Name = "Sample Event",
                Tickets = new List<Ticket>()
            };
        }

        [Test]
        public async Task GetEvent_ShouldReturnOk_WhenEventExists()
        {
            // Arrange
            var eventId = 10;
            var entity = CreateEvent(eventId);

            _eventRepoMock
                .Setup(r => r.GetEventWithTicketsAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _controller.GetEvent(eventId, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(EventResponse.FromEntity(entity));
        }

        [Test]
        public async Task GetEvent_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            var eventId = 10;

            _eventRepoMock
                .Setup(r => r.GetEventWithTicketsAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event?)null);

            var result = await _controller.GetEvent(eventId, CancellationToken.None);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
