// <copyright file="ExpenseMapper.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure.Mapping;

using ExpenseApproval.Domain;
using ExpenseApproval.Infrastructure.Stores;

/// <summary>
/// Provides methods for mapping between domain and data store representations of expenses and approval steps.
/// </summary>
public static class ExpenseMapper
{
    /// <summary>
    /// Creates a domain model instance of an expense from the specified persisted store data.
    /// </summary>
    /// <param name="store">The persisted expense data to convert to a domain model.</param>
    /// <returns>An <see cref="Expense"/> domain model populated with the data from the store.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="store"/> is null.</exception>
    public static Expense ToDomain(ExpenseStore store)
    {
        ArgumentNullException.ThrowIfNull(store, nameof(store));

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

    /// <summary>
    /// Converts an <see cref="Expense"/> domain object to its corresponding <see cref="ExpenseStore"/> data representation.
    /// </summary>
    /// <param name="domain">The <see cref="Expense"/> domain object to convert.</param>
    /// <returns>An <see cref="ExpenseStore"/> instance containing the mapped data from the specified <see cref="Expense"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="domain"/> entity is null.</exception>
    public static ExpenseStore ToStore(Expense domain)
    {
        ArgumentNullException.ThrowIfNull(domain, nameof(domain));

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

    /// <summary>
    /// Converts an <see cref="ApprovalStepStore"/> data object to its corresponding <see cref="ApprovalStep"/> domain
    /// model.
    /// </summary>
    /// <param name="store">The data object containing approval step information to be mapped to the domain model.</param>
    /// <returns>An <see cref="ApprovalStep"/> instance populated with values from the specified <paramref name="store"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="store"/> is null.</exception>
    public static ApprovalStep ToDomain(ApprovalStepStore store)
    {
        ArgumentNullException.ThrowIfNull(store, nameof(store));

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

    /// <summary>
    /// Converts an <see cref="ApprovalStep"/> domain object to its corresponding <see cref="ApprovalStepStore"/> data representation.
    /// </summary>
    /// <param name="domain">The ApprovalStep domain object to convert.</param>
    /// <returns>An ApprovalStepStore instance containing the mapped values from the specified domain object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="domain"/> entity is null.</exception>
    public static ApprovalStepStore ToStore(ApprovalStep domain)
    {
        ArgumentNullException.ThrowIfNull(domain, nameof(domain));

        return new ApprovalStepStore
        {
            Id = domain.Id,
            Role = domain.Role.ToString(),
            Order = domain.Order,
            Status = domain.Status.ToString(),
            ActionedBy = domain.ActionedBy,
            ActionedAt = domain.ActionedAt,
            RejectionReason = domain.RejectionReason,
        };
    }
}