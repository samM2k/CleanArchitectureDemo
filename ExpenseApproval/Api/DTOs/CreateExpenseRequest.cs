namespace ExpenseApproval.Api.DTOs;

public sealed record CreateExpenseRequest(Guid EmployeeId, decimal Amount, string Currency, ExpenseCategoryDto Category, string ReceiptHash);