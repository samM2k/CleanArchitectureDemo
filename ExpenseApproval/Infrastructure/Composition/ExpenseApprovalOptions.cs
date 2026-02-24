// <copyright file="ExpenseApprovalOptions.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure.Composition;

/// <summary>
/// Provides configuration options for the expense approval module's database and Entity Framework behavior.
/// </summary>
public sealed class ExpenseApprovalOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to use an in-memory SQLite database backed by a single shared open connection.
    /// </summary>
    /// <remarks>
    /// Great for demos and tests. Not intended for production.
    /// </remarks>
    public bool UseSqliteInMemory { get; set; }

    /// <summary>
    /// Gets or sets a connection string (e.g. SQLite file, SQL Server, etc.)
    /// </summary>
    /// <remarks>
    /// If not using in-memory SQLite, provide a connection string (e.g. SQLite file, SQL Server, etc.)
    /// </remarks>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the module will ensure schema is created at startup.
    /// </summary>
    /// <remarks>
    /// In production you'd usually prefer migrations (Migrate).
    /// </remarks>
    public bool EnsureCreatedOnStartup { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable EF sensitive logging in development.
    /// </summary>
    public bool EnableEfSensitiveLogging { get; set; }
}
