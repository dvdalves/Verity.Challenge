using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Application.Transaction.Events;
using Verity.Challenge.Transactions.Domain.Enums;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.UpdateTransaction;

namespace Verity.Challenge.Transactions.Application.Transaction.Handlers;

public class UpdateTransaction(TransactionsDbContext _context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<UpdateTransactionCommand, bool>
{
    public record UpdateTransactionCommand(Guid Id, decimal Amount, TransactionType Type) : IRequest<bool>;

    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction is null)
            return false;

        transaction.Update(request.Amount, request.Type);
        await _context.SaveChangesAsync(cancellationToken);

        var transactionEvent = new TransactionUpdated(request.Id, request.Amount, request.Type, DateTime.UtcNow);
        await publishEndpoint.Publish(transactionEvent, cancellationToken);

        return true;
    }
}