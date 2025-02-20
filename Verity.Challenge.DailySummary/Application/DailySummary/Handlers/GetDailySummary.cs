using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Verity.Challenge.DailySummary.Application.DTOs;
using Verity.Challenge.DailySummary.Infrastructure.Persistence;
using static Verity.Challenge.DailySummary.Application.DailySummary.Handlers.GetDailySummary;

namespace Verity.Challenge.DailySummary.Application.DailySummary.Handlers;

public class GetDailySummary(DailySummaryDbContext context, IMapper mapper) : IRequestHandler<GetDailySummaryQuery, DailySummaryDTO?>
{
    public record GetDailySummaryQuery(DateTime Date) : IRequest<DailySummaryDTO?>;

    public async Task<DailySummaryDTO?> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await context.DailySummaries
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.Date == request.Date, cancellationToken);

        return summary is null ? null : mapper.Map<DailySummaryDTO>(summary);
    }
}