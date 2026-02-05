using ExpenseApproval.Domain;

namespace ExpenseApproval.Application;

public sealed class CompanyExpensePolicy : IExpensePolicy
{
    public void EnsureExpenseAllowed(Expense expense)
    {
        if (expense.Category == ExpenseCategory.Alcohol)
            throw new DomainException("Alcohol is not reimbursable.");
    }

    public IReadOnlyList<ApproverRole> RequiredApprovals(Expense expense)
    {
        if (expense.Amount <= 500m) return new[] { ApproverRole.Manager };
        if (expense.Amount <= 2000m) return new[] { ApproverRole.Manager, ApproverRole.Finance };
        return new[] { ApproverRole.Manager, ApproverRole.Finance, ApproverRole.CFO };
    }
}