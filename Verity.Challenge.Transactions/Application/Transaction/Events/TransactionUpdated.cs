﻿using Verity.Challenge.Transactions.Domain.Enums;

namespace Verity.Challenge.Transactions.Application.Transaction.Events;

public record TransactionUpdated(Guid Id, decimal Amount, TransactionType Type, DateTime UpdatedAt);
