using ExpenseApproval.Application;
using ExpenseApproval.Domain;
using ExpenseApproval.Infrastructure.Mapping;

using Microsoft.EntityFrameworkCore;

namespace ExpenseApproval.Infrastructure.Repositories;

public sealed class EfExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _db;

    public EfExpenseRepository(AppDbContext db) => this._db = db;

    public async Task AddAsync(Expense expense, CancellationToken ct)
    {
        var expenseStore = ExpenseMapper.ToStore(expense);
        await this._db.Expenses.AddAsync(expenseStore, ct);
        await this._db.SaveChangesAsync(ct);
    }

    public async Task<Expense?> GetAsync(Guid id, CancellationToken ct)
    {
        var store = await this._db.Expenses
            .Include(e=>e.ApprovalSteps)
            .FirstOrDefaultAsync(x => x.ExpenseId == id, ct);
        return store is null ? null : ExpenseMapper.ToDomain(store);
    }

    public Task<bool> ReceiptHashExistsAsync(string receiptHash, CancellationToken ct) =>
        this._db.Expenses.AnyAsync(x => x.ReceiptHash == receiptHash, ct);

    public async Task UpdateAsync(Expense expense, CancellationToken ct)
    {
        var store = await this._db.Expenses
            .Include(x => x.ApprovalSteps)
            .FirstOrDefaultAsync(x => x.ExpenseId == expense.Id, ct);

        if (store is null)
            throw new InvalidOperationException("Cannot save: expense not found.");

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
