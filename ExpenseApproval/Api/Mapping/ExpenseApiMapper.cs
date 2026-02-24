// <copyright file="ExpenseApiMapper.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Api.Mapping;

using ExpenseApproval.Api.DTOs;
using ExpenseApproval.Application.Commands;
using ExpenseApproval.Domain;

/// <summary>
/// Provides mapping functions to convert API request objects into domain command objects for expense operations.
/// </summary>
public static class ExpenseApiMapper
{
    /// <summary>
    /// Creates a new <see cref="CreateExpenseDraftCommand "/> for drafting an expense based on the specified <see cref="CreateExpenseRequest"/> data.
    /// </summary>
    /// <param name="request">The request containing the employee ID, amount, currency, category, and receipt hash to be used for the expense
    /// draft.</param>
    /// <returns>A <see cref="CreateExpenseDraftCommand "/> instance populated with the values from the <paramref name="request"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="request"/> is null.</exception>
    public static CreateExpenseDraftCommand ToCommand(CreateExpenseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return new(
            EmployeeId: request.EmployeeId,
            Amount: request.Amount,
            Currency: request.Currency,
            Category: ToDomain(request.Category),
            ReceiptHash: request.ReceiptHash
        );
    }

    /// <summary>
    /// Creates an <see cref="ApproveExpenseCommand"/> instance representing an approval action for the specified
    /// expense.
    /// </summary>
    /// <param name="expenseId">The unique identifier of the expense to be approved.</param>
    /// <param name="request">The request containing the approver's information and role for the approval operation. Cannot be null.</param>
    /// <returns>An <see cref="ApproveExpenseCommand"/> populated with the provided expense ID, approver ID, and role.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="request"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="expenseId"/> is an empty GUID.</exception>
    public static ApproveExpenseCommand ToCommand(Guid expenseId, ApproveExpenseRequest request)
    {
        if(expenseId == Guid.Empty)
        {
            throw new ArgumentException("Expense ID cannot be empty.", nameof(expenseId));
        }

        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return new(
            ExpenseId: expenseId,
            ApproverId: request.ApproverId,
            Role: ToDomain(request.Role)
        );
    }

    /// <summary>
    /// Creates a new <see cref="RejectExpenseCommand"/> instance based on the specified expense ID and rejection request.
    /// </summary>
    /// <param name="expenseId">The unique identifier of the expense to be rejected..</param>
    /// <param name="request">The request containing details for rejecting the expense, including approver information, role, and reason.</param>
    /// <returns>A <see cref="RejectExpenseCommand"/> populated with the provided expense ID and request details.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="expenseId"/> is <see cref="Guid.Empty"/>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
    public static RejectExpenseCommand ToCommand(Guid expenseId, RejectExpenseRequest request)
    {
        if(expenseId == Guid.Empty)
        {
            throw new ArgumentException("Expense ID cannot be empty.", nameof(expenseId));
        }

        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return new(
            ExpenseId: expenseId,
            ApproverId: request.ApproverId,
            Role: ToDomain(request.Role),
            Reason: request.Reason
        );
    }

    private static ExpenseCategory ToDomain(ExpenseCategoryDto dto) => dto switch
    {
        ExpenseCategoryDto.Travel => ExpenseCategory.Travel,
        ExpenseCategoryDto.Meals => ExpenseCategory.Meals,
        ExpenseCategoryDto.OfficeSupplies => ExpenseCategory.OfficeSupplies,
        ExpenseCategoryDto.Alcohol => ExpenseCategory.Alcohol,
        ExpenseCategoryDto.Other => ExpenseCategory.Other,
        _ => ExpenseCategory.Unspecified,
    };

    private static ApproverRole ToDomain(ApproverRoleDto dto) => dto switch
    {
        ApproverRoleDto.Manager => ApproverRole.Manager,
        ApproverRoleDto.Finance => ApproverRole.Finance,
        ApproverRoleDto.CFO => ApproverRole.CFO,
        _ => ApproverRole.Unspecified,
    };
}