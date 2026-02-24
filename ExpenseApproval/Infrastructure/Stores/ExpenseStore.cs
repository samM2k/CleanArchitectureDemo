// <copyright file="ExpenseStore.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure.Stores;

/// <summary>
/// Represents an expense record, as stored in the database.
/// </summary>
public sealed class ExpenseStore
{
    /// <summary>
    /// Gets or sets the unique identifier for the expense.
    /// </summary>
    public Guid ExpenseId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the employee the expense has come from.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount associated with the transaction.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the ISO currency code used for monetary values.
    /// </summary>
    /// <remarks>The currency code should be a three-letter ISO 4217 code, such as "USD" for US dollars or "EUR" for euros.</remarks>
    public string Currency { get; set; } = default!;

    /// <summary>
    /// Gets or sets the category associated with the item as a string value.
    /// </summary>
    public string Category { get; set; } = default!;   // store enums as string (or int)

    /// <summary>
    /// Gets or sets the current status as a string value.
    /// </summary>
    public string Status { get; set; } = default!;

    /// <summary>
    /// Gets or sets the hash value representing the uploaded receipt for the expense.
    /// </summary>
    public string ReceiptHash { get; set; } = default!;

    /// <summary>
    /// Gets or sets the collection of approval steps associated with the workflow.
    /// </summary>
    public List<ApprovalStepStore> ApprovalSteps { get; set; } = default!;
}
