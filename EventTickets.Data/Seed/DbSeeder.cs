using EventTickets.Data.DataContext;
using EventTickets.Domain.Entities;
using EventTickets.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTickets.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Events.AnyAsync())
            return;

        var ev = new Event
        {
            Id = 1,
            Name = "Live Coding Lounge – Friday Night",
            StartsAt = DateTime.UtcNow.AddDays(7),
            TotalSeats = 50
        };

        for (int i = 1; i <= ev.TotalSeats; i++)
        {
            ev.Tickets.Add(new Ticket
            {
                Id = i,
                EventId = ev.Id,
                Status = TicketStatus.Available
            });
        }

        db.Events.Add(ev);
        await db.SaveChangesAsync();
    }
}
