using EventTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTickets.Data.DataContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // EVENT
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.StartsAt)
                      .IsRequired();

                entity.Property(e => e.TotalSeats)
                      .IsRequired();

                entity.HasMany(e => e.Tickets)
                      .WithOne(t => t.Event)
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TICKET
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Tickets");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.HolderName)
                      .HasMaxLength(200);

                entity.Property(t => t.Status)
                      .IsRequired();

                entity.Property(t => t.ReservedAt);

                // Optimistic concurrency token
                entity.Property(t => t.RowVersion)
                  .IsRowVersion()
                  .IsRequired(false);

            });
        }
    }
}
