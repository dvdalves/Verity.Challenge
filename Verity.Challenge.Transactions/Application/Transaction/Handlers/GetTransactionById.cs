using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Application.DTOs;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.GetTransactionById;

namespace Verity.Challenge.Transactions.Application.Transaction.Handlers;

public class GetTransactionById(TransactionsDbContext _context, IMapper _mapper) : IRequestHandler<GetTransactionByIdQuery, TransactionDTO?>
{
    public record GetTransactionByIdQuery(Guid Id) : IRequest<TransactionDTO?>;

    public async Task<TransactionDTO?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        return transaction is null ? null : _mapper.Map<TransactionDTO>(transaction);
    }
}