using ExpenseApproval.Domain;

namespace ExpenseApproval.Application.Commands;

/// <summary>
/// Represents a command to reject an expense request, specifying the expense, the approver, the approver's role, and
/// the reason for rejection.
/// </summary>
/// <param name="ExpenseId">The unique identifier of the expense to be rejected.</param>
/// <param name="ApproverId">The unique identifier of the approver performing the rejection.</param>
/// <param name="Role">The role of the approver at the time of rejection.</param>
/// <param name="Reason">The reason provided for rejecting the expense.</param>
public sealed record RejectExpenseCommand(
    Guid ExpenseId,
    Guid ApproverId,
    ApproverRole Role,
    string Reason
);