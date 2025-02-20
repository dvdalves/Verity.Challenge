using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Domain.Enums;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Commands.UpdateTransactionCommandHandler;

namespace Verity.Challenge.Transactions.Application.Commands;

public class UpdateTransactionCommandHandler(TransactionsDbContext _context) : IRequestHandler<UpdateTransactionCommand, bool>
{
    public record UpdateTransactionCommand(Guid Id, decimal Amount, TransactionType Type, DateTime UpdateTime) : IRequest<bool>;

    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction is null)
            return false;

        transaction.Update(request.Amount, request.Type, request.UpdateTime);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}