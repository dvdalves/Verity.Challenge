using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.Transactions.Application.DTOs;
using Verity.Challenge.Transactions.Infrastructure.Persistence;
using static Verity.Challenge.Transactions.Application.Queries.GetTransactionsQueryHandler;

namespace Verity.Challenge.Transactions.Application.Queries;

public class GetTransactionsQueryHandler(TransactionsDbContext _context, IMapper _mapper, IValidator<GetTransactionsQuery> _validator) : IRequestHandler<GetTransactionsQuery, List<TransactionDTO>>
{
    public record GetTransactionsQuery(DateTime? StartDate, DateTime? EndDate) : IRequest<List<TransactionDTO>>;

    public async Task<List<TransactionDTO>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = _context.Transactions.AsQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(t => t.CreatedAt >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(t => t.CreatedAt <= request.EndDate.Value);

        var transactions = await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<TransactionDTO>>(transactions);
    }
}