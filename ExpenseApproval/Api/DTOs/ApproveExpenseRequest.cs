// <copyright file="ApproveExpenseRequest.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Api.DTOs;

/// <summary>
/// Represents a request to approve an expense, including the approver's identifier and role information.
/// </summary>
/// <param name="ApproverId">The unique identifier of the user who is approving the expense.</param>
/// <param name="Role">The role of the approver, specifying their authorization level for the approval process.</param>
public sealed record ApproveExpenseRequest(Guid ApproverId, ApproverRoleDto Role);
