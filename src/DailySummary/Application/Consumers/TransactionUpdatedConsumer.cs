using Domain.Entities;
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

        var transaction = await _context.DailyTransactions
            .FirstOrDefaultAsync(t => t.Id == message.Id);

        if (transaction is null) return;

        var oldDate = transaction.Date;
        var oldAmount = transaction.Amount;
        var oldType = transaction.Type;

        var newDate = message.UpdatedAt.ToUniversalTime().Date;
        var newAmount = message.Amount;
        var newType = message.Type;

        var oldSummary = await _context.DailySummaries
            .FirstOrDefaultAsync(s => s.Date == oldDate);

        var newSummary = await _context.DailySummaries
            .FirstOrDefaultAsync(s => s.Date == newDate);

        if (oldSummary is not null)
        {
            oldSummary.Update(
                oldSummary.TotalCredits - (oldType == TransactionType.Credit ? oldAmount : 0),
                oldSummary.TotalDebits - (oldType == TransactionType.Debit ? oldAmount : 0)
            );

            if (oldSummary.TotalCredits == 0 && oldSummary.TotalDebits == 0)
            {
                _context.DailySummaries.Remove(oldSummary);
            }
        }

        transaction.Update(newAmount, newType, newDate);

        if (newSummary is null)
        {
            newSummary = DailySummaryEntity.Create(
                newDate,
                newType == TransactionType.Credit ? newAmount : 0,
                newType == TransactionType.Debit ? newAmount : 0
            );
            _context.DailySummaries.Add(newSummary);
        }
        else
        {
            newSummary.Update(
                newType == TransactionType.Credit ? newAmount : 0,
                newType == TransactionType.Debit ? newAmount : 0
            );
        }

        await _context.SaveChangesAsync();
    }
}