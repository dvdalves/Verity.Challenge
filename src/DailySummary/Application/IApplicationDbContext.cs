using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface IApplicationDbContext
{
    DbSet<DailySummaryEntity> DailySummaries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}