using FluentValidation;
using Verity.Challenge.Transactions.Application.Transaction.Handlers;

namespace Verity.Challenge.Transactions.Application.Validators;

public class GetTransactionsQueryValidator : AbstractValidator<GetTransactions.GetTransactionsQuery>
{
    public GetTransactionsQueryValidator()
    {
        RuleFor(q => q.StartDate)
            .LessThanOrEqualTo(q => q.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate.");

        RuleFor(q => q.EndDate)
            .Must(date => date == null || date <= DateTime.UtcNow)
            .WithMessage("EndDate cannot be in the future.");
    }
}
