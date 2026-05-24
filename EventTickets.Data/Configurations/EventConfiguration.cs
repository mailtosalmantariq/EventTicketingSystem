using EventTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTickets.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.TotalSeats)
                .IsRequired();
        }
    }

}
