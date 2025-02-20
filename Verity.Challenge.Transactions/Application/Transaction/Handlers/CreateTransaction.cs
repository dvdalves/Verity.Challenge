using MediatR;
using Verity.Challenge.Transactions.Domain.Entities;
using Verity.Challenge.Transactions.Domain.Enums;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.CreateTransaction;

namespace Verity.Challenge.Transactions.Application.Transaction.Handlers;

public class CreateTransaction(TransactionsDbContext context) : IRequestHandler<CreateTransactionCommand, Guid>
{
    public record CreateTransactionCommand(decimal Amount, TransactionType Type) : IRequest<Guid>;

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = TransactionEntity.Create(request.Amount, request.Type);

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}