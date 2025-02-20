using MassTransit;
using MediatR;
using Verity.Challenge.Transactions.Application.Transaction.Events;
using Verity.Challenge.Transactions.Domain.Entities;
using Verity.Challenge.Transactions.Domain.Enums;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.CreateTransaction;

namespace Verity.Challenge.Transactions.Application.Transaction.Handlers;

public class CreateTransaction(TransactionsDbContext context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateTransactionCommand, Guid>
{
    public record CreateTransactionCommand(decimal Amount, TransactionType Type) : IRequest<Guid>;

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = TransactionEntity.Create(request.Amount, request.Type);

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);

        var transactionEvent = new TransactionCreated(transaction.Id, transaction.Amount, transaction.Type, transaction.CreatedAt);
        await publishEndpoint.Publish(transactionEvent, cancellationToken);

        return transaction.Id;
    }
}