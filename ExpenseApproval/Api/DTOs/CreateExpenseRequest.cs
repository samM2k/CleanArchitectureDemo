namespace ExpenseApproval.Api.DTOs;

/// <summary>
/// Represents a request to create a new expense entry for an employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee for whom the expense is being created.</param>
/// <param name="Amount">The monetary amount of the expense.</param>
/// <param name="Currency">The ISO currency code representing the currency of the expense amount.</param>
/// <param name="Category">The category of the expense, specifying its type or purpose.</param>
/// <param name="ReceiptHash">The hash value of the associated receipt for the expense. Used to verify receipt integrity.</param>
public sealed record CreateExpenseRequest(Guid EmployeeId, decimal Amount, string Currency, ExpenseCategoryDto Category, string ReceiptHash);