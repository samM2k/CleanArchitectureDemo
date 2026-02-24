// <copyright file="ExpenseCategoryDto.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Api.DTOs;

/// <summary>
/// Specifies the available categories for expense items in financial records or reports.
/// </summary>
public enum ExpenseCategoryDto
{
    /// <summary>
    /// The default nullish value, indicating that the expense category has not been specified. This value should not be used in practice and serves as a placeholder for uninitialized states.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Represents the travel-related expense category.
    /// </summary>
    Travel = 1,

    /// <summary>
    /// Represents the meals expense category.
    /// </summary>
    Meals = 2,

    /// <summary>
    /// Represents the office supplies expense category.
    /// </summary>
    OfficeSupplies = 3,

    /// <summary>
    /// Represents the alcohol expense category.
    /// </summary>
    Alcohol = 4,

    /// <summary>
    /// Represents an unspecified or miscellaneous category.
    /// </summary>
    Other = 5,
}