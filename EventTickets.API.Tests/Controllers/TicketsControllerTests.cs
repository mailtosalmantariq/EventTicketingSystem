using EventTickets.API.Controllers;
using EventTickets.API.Models.Requests;
using EventTickets.Application.DTOs;
using EventTickets.Application.Exceptions;
using EventTickets.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventTickets.API.Tests.Controllers
{
    [TestFixture]
    public class TicketsControllerTests
    {
        private Mock<IReservationService> _reservationServiceMock;
        private Mock<IPurchaseService> _purchaseServiceMock;
        private TicketsController _controller;

        [SetUp]
        public void SetUp()
        {
            _reservationServiceMock = new Mock<IReservationService>();
            _purchaseServiceMock = new Mock<IPurchaseService>();

            _controller = new TicketsController(
                _reservationServiceMock.Object,
                _purchaseServiceMock.Object
            );
        }

        // -----------------------------
        // ReserveTicket Tests
        // -----------------------------

        [Test]
        public async Task ReserveTicket_ShouldReturnOk_WhenTicketReserved()
        {
            // Arrange
            var eventId = 1;
            var request = new ReserveTicketRequest { HolderName = "John Doe" };

            _reservationServiceMock
                .Setup(s => s.ReserveTicketAsync(
                    eventId,
                    It.IsAny<ReserveTicketRequestDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(123); // Ticket ID

            // Act
            var result = await _controller.ReserveTicket(eventId, request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new { TicketId = 123 });
        }

        [Test]
        public async Task ReserveTicket_ShouldThrowConflict_WhenNoTicketsAvailable()
        {
            // Arrange
            var eventId = 1;
            var request = new ReserveTicketRequest { HolderName = "John Doe" };

            _reservationServiceMock
                .Setup(s => s.ReserveTicketAsync(
                    eventId,
                    It.IsAny<ReserveTicketRequestDto>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("No available tickets for this event."));

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

            var request = new PurchaseTicketRequest { HolderName = "John Doe" };

            _purchaseServiceMock
                .Setup(s => s.PurchaseAsync(
                    ticketId,
                    request.HolderName,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new { TicketId = ticketId, Status = "Sold" });
        }

        [Test]
        public async Task PurchaseTicket_ShouldThrowNotFound_WhenTicketMissing()
        {
            // Arrange
            var eventId = 1;
            var ticketId = 5;
            var request = new PurchaseTicketRequest { HolderName = "John Doe" };

            _purchaseServiceMock
                .Setup(s => s.PurchaseAsync(
                    ticketId,
                    request.HolderName,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Ticket not found."));

            // Act
            var act = async () => await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Ticket not found.");
        }

        [Test]
        public async Task PurchaseTicket_ShouldThrowConflict_WhenHolderNameMismatch()
        {
            // Arrange
            var eventId = 1;
            var ticketId = 5;
            var request = new PurchaseTicketRequest { HolderName = "Wrong Name" };

            _purchaseServiceMock
                .Setup(s => s.PurchaseAsync(
                    ticketId,
                    request.HolderName,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("Ticket holder name mismatch."));

            // Act
            var act = async () => await _controller.PurchaseTicket(eventId, ticketId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Ticket holder name mismatch.");
        }
    }
}
