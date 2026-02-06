namespace ExpenseApproval.Infrastructure.Composition;

/// <summary>
/// Provides configuration options for the expense approval module's database and Entity Framework behavior.
/// </summary>
public sealed class ExpenseApprovalOptions
{
    /// <summary>
    /// Use an in-memory SQLite database backed by a single shared open connection.
    /// Great for demos and tests. Not intended for production.
    /// </summary>
    public bool UseSqliteInMemory { get; set; }

    /// <summary>
    /// If not using in-memory SQLite, provide a connection string (e.g. SQLite file, SQL Server, etc.)
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// If true, the module will ensure schema is created at startup (EnsureCreated).
    /// In production you'd usually prefer migrations (Migrate).
    /// </summary>
    public bool EnsureCreatedOnStartup { get; set; } = true;

    /// <summary>
    /// Optional: enable EF sensitive logging in dev.
    /// </summary>
    public bool EnableEfSensitiveLogging { get; set; } = false;
}
