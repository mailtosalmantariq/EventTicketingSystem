using EventTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTickets.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.Property(t => t.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(t => t.HolderName)
                .HasMaxLength(200);
        }
    }
}
