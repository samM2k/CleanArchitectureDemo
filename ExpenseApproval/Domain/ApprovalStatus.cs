// <copyright file="ApprovalStatus.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Domain;

/// <summary>
/// Specifies the status of an approval step.
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// The default nullish value, indicating that the approval status has not been specified. This value should not be used in practice and serves as a placeholder for uninitialized states.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Indicates that the approval step is pending and has not yet been actioned.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Indicates that the approval step has been actioned with rejection.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Indicates that the approval step has been been actioned with approval.
    /// </summary>
    Approved = 3,
}
