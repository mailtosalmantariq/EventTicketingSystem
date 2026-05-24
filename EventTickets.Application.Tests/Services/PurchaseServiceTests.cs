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
    public class PurchaseServiceTests
    {
        private Mock<ITicketRepository> _ticketRepoMock;
        private Mock<IUnitOfWorkRepository> _uowMock;
        private Mock<ITimeProvider> _clockMock;
        private PurchaseService _service;

        [SetUp]
        public void SetUp()
        {
            _ticketRepoMock = new Mock<ITicketRepository>();
            _uowMock = new Mock<IUnitOfWorkRepository>();
            _clockMock = new Mock<ITimeProvider>();

            _service = new PurchaseService(
                _ticketRepoMock.Object,
                _uowMock.Object,
                _clockMock.Object
            );
        }

        [Test]
        public async Task PurchaseAsync_ShouldSucceed_WhenValid()
        {
            var now = DateTime.UtcNow;
            _clockMock.Setup(c => c.UtcNow).Returns(now);

            var ticket = new Ticket
            {
                Id = 5,
                EventId = 1,
                HolderName = "John",
                Status = TicketStatus.Reserved,
                ReservedAt = now.AddMinutes(-5)
            };

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            _ticketRepoMock
                .Setup(r => r.UpdateAsync(ticket, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _service.PurchaseAsync(5, "John", CancellationToken.None);

            ticket.Status.Should().Be(TicketStatus.Sold);
            ticket.ReservedAt.Should().BeNull();
        }

        [Test]
        public async Task PurchaseAsync_ShouldThrowNotFound_WhenTicketMissing()
        {
            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ticket?)null);

            var act = async () => await _service.PurchaseAsync(5, "John", CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Ticket 5 not found.");
        }

        [Test]
        public async Task PurchaseAsync_ShouldThrowConflict_WhenReservationExpired()
        {
            var now = DateTime.UtcNow;
            _clockMock.Setup(c => c.UtcNow).Returns(now);

            var ticket = new Ticket
            {
                Id = 5,
                HolderName = "John",
                Status = TicketStatus.Reserved,
                ReservedAt = now.AddMinutes(-20)
            };

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            var act = async () => await _service.PurchaseAsync(5, "John", CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Ticket is not reserved or reservation expired.");
        }

        [Test]
        public async Task PurchaseAsync_ShouldThrowConflict_WhenHolderNameMismatch()
        {
            var now = DateTime.UtcNow;
            _clockMock.Setup(c => c.UtcNow).Returns(now);

            var ticket = new Ticket
            {
                Id = 5,
                HolderName = "Correct",
                Status = TicketStatus.Reserved,
                ReservedAt = now.AddMinutes(-5)
            };

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            var act = async () => await _service.PurchaseAsync(5, "Wrong", CancellationToken.None);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Ticket is reserved by a different holder.");
        }
    }
}
