using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.DailySummary.Handlers.GetDailySummary;

namespace Application.DailySummary.Handlers;

public class GetDailySummary(IApplicationDbContext _context, IMapper _mapper) : IRequestHandler<GetDailySummaryQuery, DailySummaryDTO?>
{
    public record GetDailySummaryQuery(DateTime Date) : IRequest<DailySummaryDTO?>;

    public async Task<DailySummaryDTO?> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _context.DailySummaries
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.Date == request.Date, cancellationToken);

        return summary is null ? null : _mapper.Map<DailySummaryDTO>(summary);
    }
}