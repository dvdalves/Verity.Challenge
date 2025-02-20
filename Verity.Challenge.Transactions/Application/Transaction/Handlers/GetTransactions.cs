using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Application.DTOs;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.GetTransactions;

namespace Verity.Challenge.Transactions.Application.Transaction.Handlers;

public class GetTransactions(TransactionsDbContext _context, IMapper _mapper) : IRequestHandler<GetTransactionsQuery, List<TransactionDTO>>
{
    public record GetTransactionsQuery() : IRequest<List<TransactionDTO>>;

    public async Task<List<TransactionDTO>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Transactions.AsQueryable();

        var transactions = await query
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<TransactionDTO>>(transactions);
    }
}