// <copyright file="ExpenseApprovalDbContext.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure;

using ExpenseApproval.Infrastructure.Stores;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the Entity Framework Core database context for the application's expense and approval workflow data.
/// </summary>
/// <remarks>This context provides access to the Expenses and ApprovalSteps tables through the corresponding DbSet
/// properties. It is intended to be used with dependency injection and configured with the appropriate database
/// provider and options. The context manages entity configuration, relationships, and schema mapping for expense
/// tracking and approval processes.</remarks>
public sealed class ExpenseApprovalDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseApprovalDbContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ExpenseApprovalDbContext(DbContextOptions<ExpenseApprovalDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the collection of expense entities in the context.
    /// </summary>
    public DbSet<ExpenseStore> Expenses => this.Set<ExpenseStore>();

    /// <summary>
    /// Gets the collection of approval step entities for querying and saving.
    /// </summary>
    public DbSet<ApprovalStepStore> ApprovalSteps => this.Set<ApprovalStepStore>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseStore>(b =>
        {
            b.ToTable("Expenses");
            b.HasKey(x => x.ExpenseId);

            b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            b.Property(x => x.Category).HasMaxLength(50).IsRequired();
            b.Property(x => x.Status).HasMaxLength(50).IsRequired();
            b.Property(x => x.ReceiptHash).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.ReceiptHash).IsUnique();

            b.HasMany(x => x.ApprovalSteps)
             .WithOne()
             .HasForeignKey(x => x.ExpenseId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApprovalStepStore>(b =>
        {
            b.ToTable("ApprovalSteps");
            b.HasKey(x => x.Id);

            b.Property(x => x.Role).HasMaxLength(50).IsRequired();
            b.Property(x => x.RejectionReason).HasMaxLength(500);
        });
    }
}
