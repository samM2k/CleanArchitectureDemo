// <copyright file="ApproverRoleDto.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Api.DTOs;

/// <summary>
/// Specifies the role of an approver in a financial approval workflow.
/// </summary>
public enum ApproverRoleDto
{
    /// <summary>
    /// The default nullish value, indicating that the approver role has not been specified. This value should not be used in practice and serves as a placeholder for uninitialized states.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Represents a manager role within the organisation.
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Represents a finance role within the organisation.
    /// </summary>
    Finance = 2,

    /// <summary>
    /// Represents the Chief Financial Officer role within the organization.
    /// </summary>
    CFO = 3,
}