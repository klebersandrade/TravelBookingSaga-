using Booking.Orchestrator.Domain.Entities;

namespace Booking.Orchestrator.Infrastructure.Repositories;

public interface IBookingSagaRepository
{
    Task<BookingSaga?> GetByIdAsync(Guid id);
    Task<BookingSaga> CreateAsync(BookingSaga saga);
    Task UpdateAsync(BookingSaga saga);
}
