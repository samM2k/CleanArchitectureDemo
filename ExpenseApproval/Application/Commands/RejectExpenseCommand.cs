using ExpenseApproval.Domain;

namespace ExpenseApproval.Application.Commands;

public sealed record RejectExpenseCommand(
    Guid ExpenseId,
    Guid ApproverId,
    ApproverRole Role,
    string Reason
);