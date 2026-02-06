using ExpenseApproval.Domain;

namespace ExpenseApproval.Application.Commands;

/// <summary>
/// Represents a command to approve an expense by a specified approver and role.
/// </summary>
/// <param name="ExpenseId">The unique identifier of the expense to be approved.</param>
/// <param name="ApproverId">The unique identifier of the user performing the approval.</param>
/// <param name="Role">The role of the approver authorizing the expense.</param>
public sealed record ApproveExpenseCommand(
    Guid ExpenseId,
    Guid ApproverId,
    ApproverRole Role
);