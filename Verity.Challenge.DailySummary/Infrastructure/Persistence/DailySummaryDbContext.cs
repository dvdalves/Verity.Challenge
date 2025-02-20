using Microsoft.EntityFrameworkCore;
using Verity.Challenge.DailySummary.Domain.Entities;

namespace Verity.Challenge.DailySummary.Infrastructure.Persistence;

public class DailySummaryDbContext(DbContextOptions<DailySummaryDbContext> options) : DbContext(options)
{
    public required DbSet<DailySummaryEntity> DailySummaries { get; set; }
}
