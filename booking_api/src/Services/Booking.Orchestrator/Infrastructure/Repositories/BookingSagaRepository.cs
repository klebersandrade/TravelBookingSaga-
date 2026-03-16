using Booking.Orchestrator.Domain.Entities;
using Booking.Orchestrator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Orchestrator.Infrastructure.Repositories;

public class BookingSagaRepository : IBookingSagaRepository
{
    private readonly BookingDbContext _dbContext;

    public BookingSagaRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingSaga?> GetByIdAsync(Guid id)
    {
        return await _dbContext.BookingSagas.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<BookingSaga> CreateAsync(BookingSaga saga)
    {
        _dbContext.BookingSagas.Add(saga);
        await _dbContext.SaveChangesAsync();
        return saga;
    }

    public async Task UpdateAsync(BookingSaga saga)
    {
        _dbContext.BookingSagas.Update(saga);
        await _dbContext.SaveChangesAsync();
    }
}
