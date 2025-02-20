using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Domain.Entities;

namespace Verity.Challenge.Transactions.Infrastructure.Persistence;

public class TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : DbContext(options)
{
    public DbSet<TransactionEntity> Transactions { get; set; } = default!;
}
