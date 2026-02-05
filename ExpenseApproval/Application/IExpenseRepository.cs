using ExpenseApproval.Domain;

namespace ExpenseApproval.Application;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken ct);
    Task<Expense?> GetAsync(Guid id, CancellationToken ct);
    Task<bool> ReceiptHashExistsAsync(string receiptHash, CancellationToken ct);
    Task UpdateAsync(Expense expense, CancellationToken ct);
}