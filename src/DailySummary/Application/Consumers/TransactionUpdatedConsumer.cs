using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Messages;

namespace Application.Consumers;

public class TransactionUpdatedConsumer(IApplicationDbContext _context) : IConsumer<TransactionUpdated>
{
    public async Task Consume(ConsumeContext<TransactionUpdated> context)
    {
        var message = context.Message;

        var updatedAtUtc = message.UpdatedAt.ToUniversalTime().Date;

        var summary = await _context.DailySummaries
            .FirstOrDefaultAsync(s => s.Date.Date == updatedAtUtc);

        var transaction = await _context.DailyTransactions
            .FirstOrDefaultAsync(t => t.Id == message.Id);

        transaction?.Update(message.Amount, message.Type);

        summary?.Update(
                summary.TotalCredits + (message.Type == TransactionType.Credit ? message.Amount : 0),
                summary.TotalDebits + (message.Type == TransactionType.Debit ? message.Amount : 0)
            );

        await _context.SaveChangesAsync();
    }
}
