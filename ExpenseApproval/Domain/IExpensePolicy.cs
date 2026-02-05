namespace ExpenseApproval.Domain;

public interface IExpensePolicy
{
    void EnsureExpenseAllowed(Expense expense);
    IReadOnlyList<ApproverRole> RequiredApprovals(Expense expense);
}
