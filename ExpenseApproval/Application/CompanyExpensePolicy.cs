using ExpenseApproval.Domain;

namespace ExpenseApproval.Application;

/// <summary>
/// A company's expense policy, an organisation-specific implementation of <see cref="IExpensePolicy"/> that defines the rules for expense validation and approval requirements based on company guidelines.
/// </summary>
/// <remarks>
/// Company policy is applied from the application layer to keep the Expense Approvals domain focused on
/// enforcing invariants and behavior. Policies are expected to evolve with organizational
/// needs, so placing them outside the domain prevents policy volatility from destabilizing
/// core domain models.
/// </remarks>
public sealed class CompanyExpensePolicy : IExpensePolicy
{
    /// <inheritdoc/>
    /// <exception cref="DomainException">Thrown if the category of the <paramref name="expense"/> is non-reimbursable, such as <see cref="ExpenseCategory.Alcohol"/>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="expense"/> is null.</exception>
    public void EnsureExpenseAllowed(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense, nameof(expense));
        if (expense.Category == ExpenseCategory.Alcohol)
            throw new DomainException("Alcohol is not reimbursable.");
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="expense"/> is null.</exception>
    public IReadOnlyList<ApproverRole> RequiredApprovals(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense, nameof(expense));
        if (expense.Amount <= 500m) return new[] { ApproverRole.Manager };
        if (expense.Amount <= 2000m) return new[] { ApproverRole.Manager, ApproverRole.Finance };
        return new[] { ApproverRole.Manager, ApproverRole.Finance, ApproverRole.CFO };
    }
}