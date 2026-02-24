// <copyright file="ExpenseStatus.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Domain;

/// <summary>
/// Specifies the status of an expense in the approval and payment workflow.
/// </summary>
public enum ExpenseStatus
{
    /// <summary>
    /// The default nullish value, indicating that the expense status has not been specified. This value should not be used in practice and serves as a placeholder for uninitialized states.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Indicates that the item is in a draft state and has not been finalized.
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Indicates that the item has been submitted for processing or review.
    /// </summary>
    Submitted = 2,

    /// <summary>
    /// Indicates that the item is currently under review.
    /// </summary>
    InReview = 3,

    /// <summary>
    /// Indicates that the request or operation has been rejected.
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Indicates that the item has been approved.
    /// </summary>
    Approved = 5,

    /// <summary>
    /// Indicates that the item has been paid.
    /// </summary>
    Paid = 6,
}
