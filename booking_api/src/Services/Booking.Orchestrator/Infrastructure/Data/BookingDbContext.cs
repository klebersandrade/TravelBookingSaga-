using Booking.Orchestrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Orchestrator.Infrastructure.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    protected BookingDbContext()
    {
    }

    public DbSet<BookingSaga> BookingSagas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orchestrator");

        modelBuilder.Entity<BookingSaga>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentState).HasConversion<string>();
            entity.Property(e => e.FailureReason).HasMaxLength(500);
        });
    }
}
