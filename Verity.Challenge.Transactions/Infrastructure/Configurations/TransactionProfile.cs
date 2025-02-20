using AutoMapper;
using Verity.Challenge.Transactions.Application.DTOs;
using Verity.Challenge.Transactions.Domain.Entities;

namespace Verity.Challenge.Transactions.Infrastructure.Configurations;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<TransactionEntity, TransactionDTO>().ReverseMap();
    }
}
