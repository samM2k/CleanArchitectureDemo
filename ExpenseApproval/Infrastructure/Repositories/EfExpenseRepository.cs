// <copyright file="EfExpenseRepository.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure.Repositories;

using ExpenseApproval.Application;
using ExpenseApproval.Domain;
using ExpenseApproval.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides an Entity Framework Core-based implementation of the <see cref="IExpenseRepository"/> interface for managing expense
/// records in the application's data store.
/// </summary>
public sealed class EfExpenseRepository : IExpenseRepository
{
    private readonly ExpenseApprovalDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfExpenseRepository"/> class using the specified database context.
    /// </summary>
    /// <param name="db">The database context to be used for data access operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="db"/> is null.</exception>
    public EfExpenseRepository(ExpenseApprovalDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db, nameof(db));
        this._db = db;
    }

    /// <summary>
    /// Asynchronously adds a new expense to the database.
    /// </summary>
    /// <param name="expense">The expense to add.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous add operation.</returns>
    public async Task AddAsync(Expense expense, CancellationToken ct)
    {
        var expenseStore = ExpenseMapper.ToStore(expense);
        await this._db.Expenses.AddAsync(expenseStore, ct);
        await this._db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Asynchronously retrieves the expense with the specified identifier, including its approval steps, if it exists.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the expense if found; otherwise,
    /// null.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="id"/> is an empty GUID.</exception>
    public async Task<Expense?> GetAsync(Guid id, CancellationToken ct)
    {
        if(id == Guid.Empty)
        {
            throw new ArgumentException("Expense ID cannot be empty.", nameof(id));
        }

        var store = await this._db.Expenses
            .Include(e => e.ApprovalSteps)
            .FirstOrDefaultAsync(x => x.ExpenseId == id, ct);
        return store is null ? null : ExpenseMapper.ToDomain(store);
    }

    /// <summary>
    /// Asynchronously determines whether an expense with the specified receipt hash exists in the database.
    /// </summary>
    /// <param name="receiptHash">The hash value of the receipt to search for.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if an expense
    /// with the specified receipt hash exists; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="receiptHash"/> is null or whitespace.</exception>
    public Task<bool> ReceiptHashExistsAsync(string receiptHash, CancellationToken ct)
    {
        if(string.IsNullOrWhiteSpace(receiptHash))
        {
            throw new ArgumentException("Rceipt hash cannot be null or whitespace.", nameof(receiptHash));
        }

        return this._db.Expenses.AnyAsync(x => x.ReceiptHash == receiptHash, ct);
    }

    /// <summary>
    /// Asynchronously updates the specified expense and its approval steps in the database.
    /// </summary>
    /// <remarks>All existing approval steps for the expense are replaced with those provided in the <paramref name="expense"/>
    /// parameter. The operation is performed as a single database transaction.</remarks>
    /// <param name="expense">The expense to update, including its updated properties and approval steps.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the update operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous update operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified expense does not exist in the database.</exception>
    /// <exception cref="ArgumentNullException">Thrown if provided <paramref name="expense"/> is null.</exception>
    public async Task UpdateAsync(Expense expense, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(expense, nameof(expense));

        var store = await this._db.Expenses
            .Include(x => x.ApprovalSteps)
            .FirstOrDefaultAsync(x => x.ExpenseId == expense.Id, ct);

        if (store is null)
        {
            throw new InvalidOperationException("Cannot save: expense not found.");
        }

        store.EmployeeId = expense.EmployeeId;
        store.Amount = expense.Amount;
        store.Currency = expense.Currency;
        store.Category = expense.Category.ToString();
        store.Status = expense.Status.ToString();
        store.ReceiptHash = expense.ReceiptHash;

        // Remove existing tracked children
        this._db.ApprovalSteps.RemoveRange(store.ApprovalSteps);
        store.ApprovalSteps.Clear();

        // Add new children and force Added state
        foreach (var step in expense.ApprovalSteps)
        {
            var stepStore = ExpenseMapper.ToStore(step);
            stepStore.ExpenseId = store.ExpenseId;

            store.ApprovalSteps.Add(stepStore);
            this._db.Entry(stepStore).State = EntityState.Added; // <-- key line
        }

        await this._db.SaveChangesAsync(ct);
    }
}
