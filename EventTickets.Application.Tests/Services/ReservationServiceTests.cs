using EventTickets.Application.DTOs;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Application.Services;
using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;
using FluentAssertions;
using Moq;

namespace EventTickets.Tests.Services
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private Mock<ITicketRepository> _ticketRepoMock;
        private Mock<IUnitOfWorkRepository> _uowMock;
        private Mock<ITimeProvider> _clockMock;
        private ReservationService _service;

        [SetUp]
        public void SetUp()
        {
            _ticketRepoMock = new Mock<ITicketRepository>();
            _uowMock = new Mock<IUnitOfWorkRepository>();
            _clockMock = new Mock<ITimeProvider>();

            _service = new ReservationService(
                _ticketRepoMock.Object,
                _uowMock.Object,
                _clockMock.Object
            );
        }

        [Test]
        public async Task ReserveTicketAsync_ShouldReturnTicketId_WhenSuccessful()
        {
            var now = DateTime.UtcNow;
            _clockMock.Setup(c => c.UtcNow).Returns(now);

            var request = new ReserveTicketRequestDto { HolderName = "John" };

            var ticket = new Ticket { Id = 10, Status = TicketStatus.Reserved };

            _ticketRepoMock
                .Setup(r => r.TryReserveAvailableTicketAsync(1, "John", now, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.ReserveTicketAsync(1, request, CancellationToken.None);

            result.Should().Be(10);
        }

        [Test]
        public async Task ReserveTicketAsync_ShouldThrowConflict_WhenNoTicketsAvailable()
        {
            var now = DateTime.UtcNow;
            _clockMock.Setup(c => c.UtcNow).Returns(now);

            var request = new ReserveTicketRequestDto { HolderName = "John" };

            _ticketRepoMock
                .Setup(r => r.TryReserveAvailableTicketAsync(1, "John", now, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ticket?)null);

            var act = async () => await _service.ReserveTicketAsync(1, request, CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("No available tickets remain for this event.");
        }
    }
}
