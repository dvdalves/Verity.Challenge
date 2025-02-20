using Application;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DailySummaryDbContext(DbContextOptions<DailySummaryDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<DailySummaryEntity> DailySummaries { get; set; } = default!;
}
