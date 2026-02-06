using ExpenseApproval.Api.DTOs;
using ExpenseApproval.Application.Commands;
using ExpenseApproval.Domain;

namespace ExpenseApproval.Api.Mapping;

public static class ExpenseApiMapper
{
    public static CreateExpenseDraftCommand ToCommand(CreateExpenseRequest req)
        => new(
            EmployeeId: req.EmployeeId,
            Amount: req.Amount,
            Currency: req.Currency,
            Category: ToDomain(req.Category),
            ReceiptHash: req.ReceiptHash
        );

    public static ApproveExpenseCommand ToCommand(Guid expenseId, ApproveExpenseRequest req)
        => new(
            ExpenseId: expenseId,
            ApproverId: req.ApproverId,
            Role: ToDomain(req.Role)
        );

    public static RejectExpenseCommand ToCommand(Guid expenseId, RejectExpenseRequest req)
        => new(
            ExpenseId: expenseId,
            ApproverId: req.ApproverId,
            Role: ToDomain(req.Role),
            Reason: req.Reason
        );

    private static ExpenseCategory ToDomain(ExpenseCategoryDto dto) => dto switch
    {
        ExpenseCategoryDto.Travel => ExpenseCategory.Travel,
        ExpenseCategoryDto.Meals => ExpenseCategory.Meals,
        ExpenseCategoryDto.OfficeSupplies => ExpenseCategory.OfficeSupplies,
        ExpenseCategoryDto.Alcohol => ExpenseCategory.Alcohol,
        ExpenseCategoryDto.Other => ExpenseCategory.Other,
        _ => ExpenseCategory.Unspecified
    };

    private static ApproverRole ToDomain(ApproverRoleDto dto) => dto switch
    {
        ApproverRoleDto.Manager => ApproverRole.Manager,
        ApproverRoleDto.Finance => ApproverRole.Finance,
        ApproverRoleDto.CFO => ApproverRole.CFO,
        _ => ApproverRole.Unspecified
    };
}