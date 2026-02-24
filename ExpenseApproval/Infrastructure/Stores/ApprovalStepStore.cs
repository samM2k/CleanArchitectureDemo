// <copyright file="ApprovalStepStore.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure.Stores;

/// <summary>
/// Represents a single approval step within an expense approval workflow, as stored in the database.
/// </summary>
public sealed class ApprovalStepStore
{
    /// <summary>
    /// Gets or sets the unique identifier for the approval step.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the parent expense.
    /// </summary>
    public Guid ExpenseId { get; set; }

    /// <summary>
    /// Gets or sets the string representation of the role required to approve this step.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the string representation of the status of the approval step.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position of the item in a sequence or collection.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who performed the action on this approval step, if it has been actioned.
    /// </summary>
    public Guid? ActionedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the action was performed on this approval step, if it has been actioned.
    /// </summary>
    public DateTimeOffset? ActionedAt { get; set; }

    /// <summary>
    /// Gets or sets the stated reason for rejection if the approval step was rejected; otherwise, it is null.
    /// </summary>
    public string? RejectionReason { get; set; }
}
