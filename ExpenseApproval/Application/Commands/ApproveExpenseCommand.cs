using ExpenseApproval.Domain;

namespace ExpenseApproval.Application.Commands;

public sealed record ApproveExpenseCommand(
    Guid ExpenseId,
    Guid ApproverId,
    ApproverRole Role
);