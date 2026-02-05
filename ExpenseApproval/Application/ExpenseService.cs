using System.Runtime.Intrinsics.Arm;

using ExpenseApproval.Application.Commands;
using ExpenseApproval.Domain;

namespace ExpenseApproval.Application;

public sealed class ExpenseService
{
    private readonly IExpenseRepository _repo;
    private readonly IExpensePolicy _policy;

    public ExpenseService(IExpenseRepository repo, IExpensePolicy policy)
    {
        this._repo = repo;
        this._policy = policy;
    }

    public async Task<Expense?> RetrieveExpenseAsync(Guid id, CancellationToken ct)
    {
        if(id == Guid.Empty)
            throw new ArgumentException("Expense ID cannot be empty.", nameof(id));

        var expense = await this._repo.GetAsync(id, ct);
        return expense;
    }

    public async Task<Guid> CreateDraftAsync(CreateExpenseDraftCommand command, CancellationToken ct)
    {
        if (await this._repo.ReceiptHashExistsAsync(command.ReceiptHash, ct))
            throw new DomainException("Duplicate receipt detected.");

        var expense = new Expense(command.EmployeeId, command.Amount, command.Currency, command.Category, command.ReceiptHash);
        await this._repo.AddAsync(expense, ct);
        return expense.Id;
    }

    public async Task SubmitAsync(Guid expenseId, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(expenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.Submit(this._policy);
        await this._repo.UpdateAsync(expense, ct);
    }

    public async Task ApproveAsync(ApproveExpenseCommand command, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(command.ExpenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.Approve(command.ApproverId, command.Role);
        await this._repo.UpdateAsync(expense, ct);
    }

    public async Task RejectAsync(RejectExpenseCommand command, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(command.ExpenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.Reject(command.ApproverId, command.Role, command.Reason);
        await this._repo.UpdateAsync(expense, ct);
    }

    public async Task MarkPaidAsync(Guid expenseId, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(expenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.MarkPaid();
        await this._repo.UpdateAsync(expense, ct);
    }
}