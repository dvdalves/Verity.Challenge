using AutoMapper;
using System.Transactions;
using Verity.Challenge.Transactions.Application.DTOs;

namespace Verity.Challenge.Transactions.Infrastructure.Configurations;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionDTO>().ReverseMap();
    }
}
