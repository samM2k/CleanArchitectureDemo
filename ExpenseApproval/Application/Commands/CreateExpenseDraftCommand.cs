// <copyright file="CreateExpenseDraftCommand.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Application.Commands;

using ExpenseApproval.Domain;

/// <summary>
/// Represents a command to create a new expense draft for an employee.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee for whom the expense draft is being created.</param>
/// <param name="Amount">The total amount of the expense.</param>
/// <param name="Currency">The ISO currency code representing the currency of the expense.</param>
/// <param name="Category">The category of the expense, such as travel, meals, or office supplies.</param>
/// <param name="ReceiptHash">A hash value representing the uploaded receipt for the expense.</param>
public sealed record CreateExpenseDraftCommand(
    Guid EmployeeId,
    decimal Amount,
    string Currency,
    ExpenseCategory Category,
    string ReceiptHash
);