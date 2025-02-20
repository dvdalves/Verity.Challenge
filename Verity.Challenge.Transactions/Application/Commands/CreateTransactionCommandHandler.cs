using MediatR;
using Verity.Challenge.Transactions.Domain.Entities;
using Verity.Challenge.Transactions.Domain.Enums;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Commands.CreateTransactionCommandHandler;

namespace Verity.Challenge.Transactions.Application.Commands;

public class CreateTransactionCommandHandler(TransactionsDbContext _context) : IRequestHandler<CreateTransactionCommand, Guid>
{
    public record CreateTransactionCommand(decimal Amount, TransactionType Type) : IRequest<Guid>;

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction(request.Amount, request.Type);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
