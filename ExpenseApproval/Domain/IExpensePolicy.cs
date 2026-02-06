namespace ExpenseApproval.Domain;

/// <summary>
/// Defines the contract for evaluating whether an expense is permitted and determining the required approval roles for
/// an expense.
/// </summary>
/// <remarks>Implementations of this interface encapsulate business rules for expense validation and approval
/// workflows. Use this interface to enforce policy checks and retrieve approval requirements before processing or
/// submitting expenses.</remarks>
public interface IExpensePolicy
{
    /// <summary>
    /// Validates whether the specified expense is permitted according to current business rules.
    /// </summary>
    /// <remarks>Throws an exception if the expense is not allowed. Use this method to enforce policy
    /// compliance before processing or recording expenses.</remarks>
    /// <param name="expense">The expense to validate.</param>
    void EnsureExpenseAllowed(Expense expense);

    /// <summary>
    /// Determines the list of approver roles required to approve the specified expense.
    /// </summary>
    /// <param name="expense">The expense for which to retrieve the required approver roles.</param>
    /// <returns>A read-only list of approver roles that must approve the given expense.</returns>
    IReadOnlyList<ApproverRole> RequiredApprovals(Expense expense);
}
