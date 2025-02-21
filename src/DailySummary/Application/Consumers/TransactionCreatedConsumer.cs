using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Messages;

namespace Application.Consumers;

public class TransactionCreatedConsumer(IApplicationDbContext _context) : IConsumer<TransactionCreated>
{
    public async Task Consume(ConsumeContext<TransactionCreated> context)
    {
        var message = context.Message;

        var existingTransaction = await _context.DailyTransactions
            .AnyAsync(t => t.Id == message.Id);

        if (!existingTransaction)
        {
            var transaction = DailyTransactionEntity.Create(message.Id, message.CreatedAt, message.Amount, message.Type);
            _context.DailyTransactions.Add(transaction);

            var summary = await _context.DailySummaries
                .FirstOrDefaultAsync(s => s.Date == message.CreatedAt.Date);

            if (summary == null)
            {
                summary = DailySummaryEntity.Create(
                    message.CreatedAt.Date,
                    message.Type == TransactionType.Credit ? message.Amount : 0,
                    message.Type == TransactionType.Debit ? message.Amount : 0
                );

                _context.DailySummaries.Add(summary);
            }
            else
            {
                summary.Update(
                    summary.TotalCredits + (message.Type == TransactionType.Credit ? message.Amount : 0),
                    summary.TotalDebits + (message.Type == TransactionType.Debit ? message.Amount : 0)
                );
            }

            await _context.SaveChangesAsync();
        }
    }
}