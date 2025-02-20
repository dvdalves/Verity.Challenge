using Application.Transaction.Events;
using Domain.Entities;
using Domain.Enums;
using MassTransit;
using MediatR;
using static Application.Transaction.Handlers.CreateTransaction;

namespace Application.Transaction.Handlers;

public class CreateTransaction(IApplicationDbContext _context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateTransactionCommand, Guid>
{
    public record CreateTransactionCommand(decimal Amount, TransactionType Type) : IRequest<Guid>;

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = TransactionEntity.Create(request.Amount, request.Type);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        var transactionEvent = new TransactionCreated(transaction.Id, transaction.Amount, transaction.Type, transaction.CreatedAt);
        await publishEndpoint.Publish(transactionEvent, cancellationToken);

        return transaction.Id;
    }
}