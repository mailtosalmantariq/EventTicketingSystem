using EventTickets.API.Controllers;
using EventTickets.Application.DTOs;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventTickets.API.Tests.Controllers
{
    [TestFixture]
    public class EventsControllerTests
    {
        private Mock<IEventService> _eventServiceMock;
        private EventsController _controller;

        [SetUp]
        public void SetUp()
        {
            _eventServiceMock = new Mock<IEventService>();
            _controller = new EventsController(_eventServiceMock.Object);
        }

        [Test]
        public async Task GetEvent_ShouldReturnOk_WhenEventExists()
        {
            // Arrange
            var eventId = 10;

            var dto = new EventDetailsDto(
                id: eventId,
                name: "Sample Event",
                startsAt: DateTime.UtcNow,
                available: 5,
                reserved: 2,
                sold: 3
            );

            _eventServiceMock
                .Setup(s => s.GetEventDetailsAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.GetEvent(eventId, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(dto);
        }

        [Test]
        public async Task GetEvent_ShouldThrowNotFound_WhenServiceThrowsNotFoundException()
        {
            // Arrange
            var eventId = 10;

            _eventServiceMock
                .Setup(s => s.GetEventDetailsAsync(eventId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Event not found"));

            // Act
            Func<Task> act = async () => await _controller.GetEvent(eventId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
