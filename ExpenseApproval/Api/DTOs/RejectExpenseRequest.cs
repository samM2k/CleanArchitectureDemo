namespace ExpenseApproval.Api.DTOs;

public sealed record RejectExpenseRequest(Guid ApproverId, ApproverRoleDto Role, string Reason);