using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Domain.Entities;

namespace Verity.Challenge.Transactions.Infrastructure.Persistence;

public class TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : DbContext(options)
{
    public required DbSet<TransactionEntity> Transactions { get; set; }
}
