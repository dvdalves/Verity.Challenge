using AutoMapper;
using Verity.Challenge.DailySummary.Application.DTOs;
using Verity.Challenge.DailySummary.Domain.Entities;

namespace Verity.Challenge.DailySummary.Infrastructure.Configurations;

public class DailySummaryProfile : Profile
{
    public DailySummaryProfile()
    {
        CreateMap<DailySummaryEntity, DailySummaryDTO>().ReverseMap();
    }
}