using Domain.Enums;

namespace Application.Transaction.Events;

public record TransactionUpdated(Guid Id, decimal Amount, TransactionType Type, DateTime UpdatedAt);
