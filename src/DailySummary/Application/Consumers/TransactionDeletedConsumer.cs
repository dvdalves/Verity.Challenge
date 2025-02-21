using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Messages;

namespace Application.Consumers;

public class TransactionDeletedConsumer(IApplicationDbContext _context) : IConsumer<TransactionDeleted>
{
    public async Task Consume(ConsumeContext<TransactionDeleted> context)
    {
        var message = context.Message;

        var transaction = await _context.DailySummaries
            .FirstOrDefaultAsync(s => s.Id == message.Id);

        if (transaction != null)
        {
            _context.DailySummaries.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
