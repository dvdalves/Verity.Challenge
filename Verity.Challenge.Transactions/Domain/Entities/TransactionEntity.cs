﻿using Verity.Challenge.Transactions.Domain.Enums;

namespace Verity.Challenge.Transactions.Domain.Entities;

public class TransactionEntity
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; } = null;

    private TransactionEntity() { }

    public static TransactionEntity Create(decimal amount, TransactionType type)
    {
        return amount <= 0
            ? throw new TransactionDomainException("Amount must be greater than zero.")
            : new TransactionEntity
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };
    }

    public void Update(decimal amount, TransactionType type)
    {
        if (amount <= 0)
            throw new TransactionDomainException("Amount must be greater than zero.");

        Amount = amount;
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }
}