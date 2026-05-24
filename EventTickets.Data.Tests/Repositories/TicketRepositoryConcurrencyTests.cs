using EventTickets.Data.DataContext;
using EventTickets.Data.Repositories;
using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventTickets.Data.Tests.Repositories
{
    public class TicketRepositoryConcurrencyTests
    {
        private DbContextOptions<AppDbContext> _options = null!;
        private SqliteConnection _connection = null!;

        [SetUp]
        public async Task Setup()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            await _connection.OpenAsync();

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            await using var db = new AppDbContext(_options);
            await db.Database.EnsureCreatedAsync();

            // Insert Event FIRST
            db.Events.Add(new Event
            {
                Name = "Test Event",
                StartsAt = DateTime.UtcNow,
                TotalSeats = 1
            });
            await db.SaveChangesAsync();

            // Insert Ticket referencing EventId = 1
            db.Tickets.Add(new Ticket
            {
                EventId = 1,
                Status = TicketStatus.Available
            });
            await db.SaveChangesAsync();
        }


        [TearDown]
        public async Task Teardown()
        {
            await _connection.CloseAsync();
        }

        [Test]
        public async Task OnlyOneReservationShouldSucceed_WhenTwoRequestsRunConcurrently()
        {
            // Arrange
            var repo1 = new TicketRepository(new AppDbContext(_options));
            var repo2 = new TicketRepository(new AppDbContext(_options));

            // Act — run two reservations at the same time
            var now = DateTime.UtcNow;

            var t1 = repo1.TryReserveAvailableTicketAsync(1, "UserA", now, CancellationToken.None);
            var t2 = repo2.TryReserveAvailableTicketAsync(1, "UserB", now, CancellationToken.None);

            var results = await Task.WhenAll(t1, t2);

            // Assert — exactly ONE succeeds
            var successCount = results.Count(r => r != null);
            Assert.That(successCount, Is.EqualTo(1), "Only one reservation should succeed");

            // Assert — DB should contain exactly one reserved ticket
            await using var db = new AppDbContext(_options);
            var reservedCount = await db.Tickets.CountAsync(t => t.Status == TicketStatus.Reserved);
            Assert.That(reservedCount, Is.EqualTo(1));
        }

        [Test]
        public async Task SecondReservationShouldFail_WhenRowVersionHasChanged()
        {
            // Arrange
            var ctx1 = new AppDbContext(_options);
            var ctx2 = new AppDbContext(_options);

            var repo1 = new TicketRepository(ctx1);
            var repo2 = new TicketRepository(ctx2);

            var now = DateTime.UtcNow;

            // First reservation succeeds
            var first = await repo1.TryReserveAvailableTicketAsync(1, "UserA", now, CancellationToken.None);
            Assert.That(first, Is.Not.Null);

            // Second reservation should fail due to concurrency
            var second = await repo2.TryReserveAvailableTicketAsync(1, "UserB", now, CancellationToken.None);
            Assert.That(second, Is.Null, "Second reservation must fail due to RowVersion mismatch");
        }

        [Test]
        public async Task TicketShouldBeReservedExactlyOnce()
        {
            // Arrange
            var repo = new TicketRepository(new AppDbContext(_options));

            var now = DateTime.UtcNow;

            // Act
            var result = await repo.TryReserveAvailableTicketAsync(1, "Salman", now, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);

            await using var db = new AppDbContext(_options);
            var reservedCount = await db.Tickets.CountAsync(t => t.Status == TicketStatus.Reserved);
            Assert.That(reservedCount, Is.EqualTo(1));
        }
    }
}
