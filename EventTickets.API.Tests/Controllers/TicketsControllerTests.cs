using EventTickets.API.Controllers;
using EventTickets.API.Models.Requests;
using EventTickets.API.Models.Responses;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventTickets.API.Tests.Controllers
{
    [TestFixture]
    public class TicketsControllerTests
    {
        private Mock<ITicketRepository> _ticketRepoMock;
        private Mock<IUnitOfWorkRepository> _unitOfWorkMock;
        private Mock<ITimeProvider> _timeProviderMock;
        private TicketsController _controller;

        [SetUp]
        public void SetUp()
        {
            _ticketRepoMock = new Mock<ITicketRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWorkRepository>();
            _timeProviderMock = new Mock<ITimeProvider>();

            _controller = new TicketsController(
                _ticketRepoMock.Object,
                _unitOfWorkMock.Object,
                _timeProviderMock.Object
            );
        }
        private Ticket CreateReservedTicket(int eventId, int ticketId = 1)
        {
            return new Ticket
            {
                Id = ticketId,
                EventId = eventId,
                HolderName = "John Doe",
                Status = TicketStatus.Reserved
            };
        }

        private Ticket CreateSoldTicket(int eventId, int ticketId = 1)
        {
            return new Ticket
            {
                Id = ticketId,
                EventId = eventId,
                HolderName = "John Doe",
                Status = TicketStatus.Sold
            };
        }

        // -----------------------------
        // ReserveTicket Tests
        // -----------------------------

        [Test]
        public async Task ReserveTicket_ShouldReturnOk_WhenTicketReserved()
        {
            // Arrange
            var eventId = 1;
            var now = DateTime.UtcNow;

            var request = new ReserveTicketRequest
            {
                HolderName = "John Doe"
            };

            var ticket = CreateReservedTicket(eventId);

            _timeProviderMock.Setup(t => t.UtcNow).Returns(now);

            _ticketRepoMock
                .Setup(r => r.TryReserveAvailableTicketAsync(
                    eventId,
                    request.HolderName,
                    now,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            // Act
            var result = await _controller.ReserveTicket(eventId, request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(TicketResponse.FromEntity(ticket));
        }

        [Test]
        public async Task ReserveTicket_ShouldThrowConflict_WhenNoTicketsAvailable()
        {
            // Arrange
            var eventId = 1;
            var now = DateTime.UtcNow;

            var request = new ReserveTicketRequest
            {
                HolderName = "John Doe"
            };

            _timeProviderMock.Setup(t => t.UtcNow).Returns(now);

            _ticketRepoMock
                .Setup(r => r.TryReserveAvailableTicketAsync(
                    eventId,
                    request.HolderName,
                    now,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ticket?)null);

            // Act
            var act = async () => await _controller.ReserveTicket(eventId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("No available tickets for this event.");
        }

        // -----------------------------
        // PurchaseTicket Tests
        // -----------------------------

        [Test]
        public async Task PurchaseTicket_ShouldReturnOk_WhenValid()
        {
            // Arrange
            var eventId = 1;
            var ticketId = 5;

            var request = new PurchaseTicketRequest
            {
                HolderName = "John Doe"
            };

            var ticket = CreateReservedTicket(eventId, ticketId);

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(ticketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(TicketResponse.FromEntity(ticket));
        }

        [Test]
        public async Task PurchaseTicket_ShouldThrowNotFound_WhenTicketMissing()
        {
            // Arrange
            var eventId = 1;
            var ticketId = 5;

            var request = new PurchaseTicketRequest
            {
                HolderName = "John Doe"
            };

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(ticketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ticket?)null);

            // Act
            var act = async () => await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Ticket not found.");
        }

        [Test]
        public async Task PurchaseTicket_ShouldThrowConflict_WhenTicketNotReserved()
        {
            // Arrange
            var eventId = 1;
            var ticketId = 5;

            var request = new PurchaseTicketRequest
            {
                HolderName = "John Doe"
            };

            var ticket = CreateSoldTicket(eventId, ticketId);

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(ticketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            // Act
            var act = async () => await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Ticket is not reserved.");
        }

        [Test]
        public async Task PurchaseTicket_ShouldThrowConflict_WhenHolderNameMismatch()
        {
            // Arrange
            var eventId = 1;
            var ticketId = 5;

            var request = new PurchaseTicketRequest
            {
                HolderName = "Wrong Name"
            };

            var ticket = CreateReservedTicket(eventId, ticketId);
            ticket.HolderName = "Correct Name";

            _ticketRepoMock
                .Setup(r => r.GetTicketByIdAsync(ticketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            // Act
            var act = async () => await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Ticket holder name mismatch.");
        }
    }
}
