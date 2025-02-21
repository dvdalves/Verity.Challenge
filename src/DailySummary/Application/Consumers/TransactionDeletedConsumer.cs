using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Messages;

namespace Application.Consumers;

public class TransactionDeletedConsumer(IApplicationDbContext _context) : IConsumer<TransactionDeleted>
{
    public async Task Consume(ConsumeContext<TransactionDeleted> context)
    {
        var message = context.Message;

        // Verificar se a transação realmente existiu no histórico
        var transaction = await _context.DailyTransactions
            .FirstOrDefaultAsync(t => t.Id == message.Id);

        if (transaction != null)
        {
            // Encontrar o resumo diário baseado na data da transação deletada
            var summary = await _context.DailySummaries
                .FirstOrDefaultAsync(s => s.Date == transaction.Date);

            if (summary != null)
            {
                if (transaction.Type == TransactionType.Credit)
                    summary.Update(summary.TotalCredits - transaction.Amount, summary.TotalDebits);
                else
                    summary.Update(summary.TotalCredits, summary.TotalDebits - transaction.Amount);

                // Se o saldo for zero, podemos optar por remover o resumo
                if (summary.TotalCredits == 0 && summary.TotalDebits == 0)
                    _context.DailySummaries.Remove(summary);

                // Remover a transação do histórico
                _context.DailyTransactions.Remove(transaction);

                await _context.SaveChangesAsync();
            }
        }
    }
}
