using Verity.Challenge.Transactions.Domain.Enums;

namespace Verity.Challenge.Transactions.Application.DTOs;

public class TransactionDTO
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}