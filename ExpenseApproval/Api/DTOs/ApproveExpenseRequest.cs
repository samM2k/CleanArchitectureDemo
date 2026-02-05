namespace ExpenseApproval.Api.DTOs;

public sealed record ApproveExpenseRequest(Guid ApproverId, ApproverRoleDto Role);
