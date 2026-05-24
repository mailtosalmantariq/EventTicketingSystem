using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Services;
using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;
using FluentAssertions;
using Moq;

namespace EventTickets.Tests.Services
{
    [TestFixture]
    public class EventServiceTests
    {
        private Mock<IEventRepository> _eventRepoMock;
        private Mock<ITimeProvider> _clockMock;
        private EventService _service;

        [SetUp]
        public void SetUp()
        {
            _eventRepoMock = new Mock<IEventRepository>();
            _clockMock = new Mock<ITimeProvider>();

            _service = new EventService(_eventRepoMock.Object, _clockMock.Object);
        }

        [Test]
        public async Task GetEventDetailsAsync_ShouldReturnCountsCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _clockMock.Setup(c => c.UtcNow).Returns(now);

            var ev = new Event
            {
                Id = 1,
                Name = "Concert",
                StartsAt = now.AddDays(1),
                Tickets = new List<Ticket>
                {
                    new Ticket { Status = TicketStatus.Available },
                    new Ticket { Status = TicketStatus.Reserved, ReservedAt = now.AddMinutes(-20) }, // expired
                    new Ticket { Status = TicketStatus.Reserved, ReservedAt = now.AddMinutes(-5) },  // active
                    new Ticket { Status = TicketStatus.Sold }
                }
            };

            _eventRepoMock
                .Setup(r => r.GetEventWithTicketsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ev);

            // Act
            var result = await _service.GetEventDetailsAsync(1, CancellationToken.None);

            // Assert
            result.Available.Should().Be(2); // available + expired reserved
            result.Reserved.Should().Be(1);  // active reserved
            result.Sold.Should().Be(1);
        }

        [Test]
        public async Task GetEventDetailsAsync_ShouldThrowNotFound_WhenEventMissing()
        {
            _eventRepoMock
                .Setup(r => r.GetEventWithTicketsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event?)null);

            var act = async () => await _service.GetEventDetailsAsync(1, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Event 1 not found.");
        }
    }
}
