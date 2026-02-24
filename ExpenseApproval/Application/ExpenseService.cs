// <copyright file="ExpenseService.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Application;

using ExpenseApproval.Application.Commands;
using ExpenseApproval.Domain;

/// <summary>
/// An application level service for orchestrating operations related to expense management, including creation, submission, approval, rejection, and payment processing.
/// </summary>
/// <remarks>
/// This service acts as a facade that encapsulates the interactions between the domain models and the data persistence layer.
/// </remarks>
public sealed class ExpenseService
{
    private readonly IExpenseRepository _repo;
    private readonly IExpensePolicy _policy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseService"/> class with the specified repository and policy dependencies.
    /// </summary>
    /// <param name="repo">The repository for performing CRUD operations on expenses.</param>
    /// <param name="policy">The policy for validating expenses and determining approval requirements.</param>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="repo"/> or <paramref name="policy"/> is <see langword="null"/>.</exception>
    public ExpenseService(IExpenseRepository repo, IExpensePolicy policy)
    {
        this._repo = repo ?? throw new ArgumentNullException(nameof(repo));
        this._policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    /// <summary>
    /// Retrieves an expense by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to retrieve.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the retrieved <see cref="Expense"/> or <see langword="null"/> if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the Expense ID is empty.</exception>
    public async Task<Expense?> RetrieveExpenseAsync(Guid id, CancellationToken ct)
    {
        if(id == Guid.Empty)
        {
            throw new ArgumentException("Expense ID cannot be empty.", nameof(id));
        }

        var expense = await this._repo.GetAsync(id, ct);
        return expense;
    }

    /// <summary>
    /// Creates a new expense draft based on the specified command.
    /// </summary>
    /// <param name="command">The command containing the details required to create the expense draft.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Guid"/> representing the unique identifier of the newly created expense draft.</returns>
    /// <exception cref="DomainException">Thrown if an expense draft with the same receipt hash already exists.</exception>
    public async Task<Guid> CreateDraftAsync(CreateExpenseDraftCommand command, CancellationToken ct)
    {
        if (await this._repo.ReceiptHashExistsAsync(command.ReceiptHash, ct))
        {
            throw new DomainException("Duplicate receipt detected.");
        }

        var expense = new Expense(command.EmployeeId, command.Amount, command.Currency, command.Category, command.ReceiptHash);
        await this._repo.AddAsync(expense, ct);
        return expense.Id;
    }

    /// <summary>
    /// Submits the specified expense for approval asynchronously.
    /// </summary>
    /// <param name="expenseId">The unique identifier of the expense to submit.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous submit operation.</returns>
    /// <exception cref="DomainException">Thrown if an expense with the specified identifier does not exist.</exception>
    public async Task SubmitAsync(Guid expenseId, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(expenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.Submit(this._policy);
        await this._repo.UpdateAsync(expense, ct);
    }

    /// <summary>
    /// Approves the specified expense asynchronously.
    /// </summary>
    /// <param name="command">The command containing the details required to approve the expense, including the expense identifier, approver,
    /// and role.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous approve operation.</returns>
    /// <exception cref="DomainException">Thrown if the specified expense is not found.</exception>
    public async Task ApproveAsync(ApproveExpenseCommand command, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(command.ExpenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.Approve(command.ApproverId, command.Role);
        await this._repo.UpdateAsync(expense, ct);
    }

    /// <summary>
    /// Rejects the specified expense asynchronously based on the provided command.
    /// </summary>
    /// <param name="command">The command containing information required to reject the expense, including the expense identifier, approver
    /// details, and rejection reason.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous reject operation.</returns>
    /// <exception cref="DomainException">Thrown if the specified expense does not exist.</exception>
    public async Task RejectAsync(RejectExpenseCommand command, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(command.ExpenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.Reject(command.ApproverId, command.Role, command.Reason);
        await this._repo.UpdateAsync(expense, ct);
    }

    /// <summary>
    /// Marks the specified expense as paid asynchronously.
    /// </summary>
    /// <param name="expenseId">The unique identifier of the expense to mark as paid.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    /// <exception cref="DomainException">Thrown if an expense with the specified identifier is not found.</exception>
    public async Task MarkPaidAsync(Guid expenseId, CancellationToken ct)
    {
        var expense = await this._repo.GetAsync(expenseId, ct) ?? throw new DomainException("Expense not found.");
        expense.MarkPaid();
        await this._repo.UpdateAsync(expense, ct);
    }
}