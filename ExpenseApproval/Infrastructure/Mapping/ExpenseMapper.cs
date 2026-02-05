using ExpenseApproval.Domain;
using ExpenseApproval.Infrastructure.Stores;

namespace ExpenseApproval.Infrastructure.Mapping;

public static class ExpenseMapper
{
    public static Expense ToDomain(ExpenseStore store)
    {
        // You said you’re going get-only + ctor-set domain models.
        // That means you’ll need a domain constructor/factory that accepts all persisted state.
        // Example assumes you have something like Expense.Rehydrate(...)
        var steps = store.ApprovalSteps
            .OrderBy(s => s.Order)
            .Select(ToDomain)
            .ToList();

        return new Expense(
            id: store.ExpenseId,
            employeeId: store.EmployeeId,
            amount: store.Amount,
            currency: store.Currency,
            category: Enum.Parse<ExpenseCategory>(store.Category, ignoreCase: true),
            receiptHash: store.ReceiptHash,
            status: Enum.Parse<ExpenseStatus>(store.Status, ignoreCase: true),
            steps: steps
        );
    }

    public static ExpenseStore ToStore(Expense domain)
    {
        var store = new ExpenseStore
        {
            ExpenseId = domain.Id,
            EmployeeId = domain.EmployeeId,
            Amount = domain.Amount,
            Currency = domain.Currency,
            Category = domain.Category.ToString(),
            Status = domain.Status.ToString(),
            ReceiptHash = domain.ReceiptHash,
            ApprovalSteps = domain.ApprovalSteps.Select(ToStore).ToList(),
        };

        return store;
    }

    public static ApprovalStep ToDomain(ApprovalStepStore store)
    {
        return new ApprovalStep(
            id: store.Id,
            role: Enum.Parse<ApproverRole>(store.Role, ignoreCase: true),
            order: store.Order,
            status: Enum.Parse<ApprovalStatus>(store.Status, ignoreCase: true),
            actionedBy: store.ActionedBy,
            actionedAt: store.ActionedAt,
            rejectionReason: store.RejectionReason
        );
    }

    public static ApprovalStepStore ToStore(ApprovalStep domain)
    {
        return new ApprovalStepStore
        {
            Id = domain.Id,
            Role = domain.Role.ToString(),
            Order = domain.Order,
            Status = domain.Status.ToString(),
            ActionedBy = domain.ActionedBy,
            ActionedAt = domain.ActionedAt,
            RejectionReason = domain.RejectionReason
        };
    }
}