using ExpenseApproval.Domain;

namespace ExpenseApproval.Application.Commands;

public sealed record CreateExpenseDraftCommand(
    Guid EmployeeId,
    decimal Amount,
    string Currency,
    ExpenseCategory Category,
    string ReceiptHash
);