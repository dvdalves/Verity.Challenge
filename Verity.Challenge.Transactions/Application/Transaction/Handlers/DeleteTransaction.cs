using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.DeleteTransaction;

namespace Verity.Challenge.Transactions.Application.Transaction.Handlers;

public class DeleteTransaction(TransactionsDbContext _context) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public record DeleteTransactionCommand(Guid Id) : IRequest<bool>;

    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction is null)
            return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}