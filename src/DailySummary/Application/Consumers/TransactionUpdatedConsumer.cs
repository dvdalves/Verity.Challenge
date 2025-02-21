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

        var summary = await _context.DailySummaries
            .FirstOrDefaultAsync(s => s.Date == message.UpdatedAt.Date);

        if (summary != null)
        {
            summary.Update(
                summary.TotalCredits + (message.Type == TransactionType.Credit ? message.Amount : 0),
                summary.TotalDebits + (message.Type == TransactionType.Debit ? message.Amount : 0)
            );

            await _context.SaveChangesAsync();
        }
    }
}
