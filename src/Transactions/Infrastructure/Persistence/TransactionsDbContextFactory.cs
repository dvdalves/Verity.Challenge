using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class TransactionsDbContextFactory : IDesignTimeDbContextFactory<TransactionsDbContext>
{
    public TransactionsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TransactionsDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=verity_transactions;Username=admin;Password=admin");

        return new TransactionsDbContext(optionsBuilder.Options);
    }
}
