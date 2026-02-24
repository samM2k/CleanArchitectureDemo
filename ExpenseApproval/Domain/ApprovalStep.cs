// <copyright file="ApprovalStep.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Domain;

/// <summary>
/// Represents a single step in an approval workflow, including its assigned role, order, status, and action details.
/// </summary>
/// <remarks>An approval step tracks the progress and outcome of a specific stage in a multi-step approval
/// process. Each step is associated with an approver role and maintains information about its status, who actioned it,
/// and when. Use the provided methods to mark the step as approved or rejected. Instances are immutable except for
/// status and action details, which are updated when the step is actioned.</remarks>
public sealed class ApprovalStep
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApprovalStep"/> class with the specified approver role, order, status, and
    /// related action details.
    /// </summary>
    /// <param name="id">The unique identifier for the approval step.</param>
    /// <param name="role">The role of the approver responsible for this step.</param>
    /// <param name="order">The sequence order of this approval step within the overall approval process.</param>
    /// <param name="status">The current status of the approval step.</param>
    /// <param name="actionedBy">The identifier of the user who actioned this step, or null if not yet actioned.</param>
    /// <param name="actionedAt">The date and time when this step was actioned, or null if not yet actioned.</param>
    /// <param name="rejectionReason">The reason for rejection, if the step was rejected; otherwise, null.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="id"/> is empty, <paramref name="role"/> is <see cref="ApproverRole.Unspecified"/> or <paramref name="order"/> is negative.</exception>
    public ApprovalStep(Guid id, ApproverRole role, int order, ApprovalStatus status, Guid? actionedBy, DateTimeOffset? actionedAt, string? rejectionReason)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Id is required.");
        }

        if (role == ApproverRole.Unspecified)
        {
            throw new DomainException("Role is required.");
        }

        if (order < 0)
        {
            throw new DomainException("Order must be non-negative.");
        }

        this.Id = id;
        this.Role = role;
        this.Order = order;
        this.Status = status;
        this.ActionedBy = actionedBy;
        this.ActionedAt = actionedAt;
        this.RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApprovalStep"/> class with the specified approver role and step order, setting
    /// the status to pending.
    /// </summary>
    /// <param name="role">The role of the approver responsible for this approval step.</param>
    /// <param name="order">The sequence order of this step within the approval process.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="role"/> is <see cref="ApproverRole.Unspecified"/> or <paramref name="order"/> is negative.</exception>
    public ApprovalStep(ApproverRole role, int order)
        : this(Guid.NewGuid(), role, order, ApprovalStatus.Pending, null, null, null)
    {
    }

    /// <summary>
    /// Gets the unique identifier for the approval step.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the role responsible for approving this step. This determines who can action this step in the workflow.
    /// </summary>
    public ApproverRole Role { get; }

    /// <summary>
    /// Gets the zero-based index that determines the position of the item within a collection or sequence.
    /// </summary>
    /// <remarks>The order property is used to define the sequence of approval steps. Steps with lower order values are processed before those with higher values.</remarks>
    public int Order { get; }

    /// <summary>
    /// Gets the current approval status of the item.
    /// </summary>
    public ApprovalStatus Status { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who performed the action, if available.
    /// </summary>
    public Guid? ActionedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the action was completed.
    /// </summary>
    public DateTimeOffset? ActionedAt { get; private set; }

    /// <summary>
    /// Gets the reason provided for rejecting the request or operation.
    /// </summary>
    public string? RejectionReason { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the item has been approved.
    /// </summary>
    public bool IsApproved => this.Status == ApprovalStatus.Approved;

    /// <summary>
    /// Gets a value indicating whether the approval has been rejected.
    /// </summary>
    public bool IsRejected => this.Status == ApprovalStatus.Rejected;

    /// <summary>
    /// Marks the approval step as approved by the specified approver.
    /// </summary>
    /// <param name="approverId">The unique identifier of the user who is approving the step.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="approverId"/> is empty or if the approval step has already been actioned.</exception>
    public void MarkApproved(Guid approverId)
    {
        if (approverId == Guid.Empty)
        {
            throw new DomainException("ApproverId is required.");
        }

        if (this.Status != ApprovalStatus.Pending)
        {
            throw new DomainException("Step already actioned.");
        }

        this.Status = ApprovalStatus.Approved;
        this.ActionedBy = approverId;
        this.ActionedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Marks the approval step as rejected by the specified approver, recording the provided reason.
    /// </summary>
    /// <param name="approverId">The unique identifier of the approver performing the rejection.</param>
    /// <param name="reason">The reason for rejecting the approval step.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="approverId"/> is empty, <paramref name="reason"/> is null, empty, or whitespace, or if
    /// the approval step has already been actioned.</exception>
    public void MarkRejected(Guid approverId, string reason)
    {
        if (approverId == Guid.Empty)
        {
            throw new DomainException("ApproverId is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("Reason is required.");
        }

        if (this.Status != ApprovalStatus.Pending)
        {
            throw new DomainException("Step already actioned.");
        }

        this.Status = ApprovalStatus.Rejected;
        this.RejectionReason = reason.Trim();
        this.ActionedBy = approverId;
        this.ActionedAt = DateTimeOffset.UtcNow;
    }
}
