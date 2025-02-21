using Application.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Application.DailySummary.Handlers.GetDailySummary;

namespace Application.DailySummary.Handlers;

public class GetDailySummary(IApplicationDbContext _context, IMapper _mapper) : IRequestHandler<GetDailySummaryQuery, DailySummaryDTO>
{
    public record GetDailySummaryQuery(DateTime Date) : IRequest<DailySummaryDTO>;

    public async Task<DailySummaryDTO> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
    {
        var dateUtc = DateTime.SpecifyKind(request.Date.Date, DateTimeKind.Utc);

        var summary = await _context.DailySummaries
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Date == dateUtc, cancellationToken);

        return summary is null
            ? new DailySummaryDTO { Date = dateUtc, TotalCredits = 0, TotalDebits = 0, Balance = 0 }
            : _mapper.Map<DailySummaryDTO>(summary);
    }
}