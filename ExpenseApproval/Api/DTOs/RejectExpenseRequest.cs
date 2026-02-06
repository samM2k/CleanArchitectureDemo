namespace ExpenseApproval.Api.DTOs;

/// <summary>
/// Represents a request to reject an expense, including the approver's identity, role, and the reason for rejection.
/// </summary>
/// <param name="ApproverId">The unique identifier of the approver who is rejecting the expense.</param>
/// <param name="Role">The role of the approver within the approval workflow.</param>
/// <param name="Reason">The explanation provided for rejecting the expense.</param>
public sealed record RejectExpenseRequest(Guid ApproverId, ApproverRoleDto Role, string Reason);