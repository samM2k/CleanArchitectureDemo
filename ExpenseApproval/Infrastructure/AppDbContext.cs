using ExpenseApproval.Infrastructure.Stores;

using Microsoft.EntityFrameworkCore;

namespace ExpenseApproval.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public DbSet<ExpenseStore> Expenses => this.Set<ExpenseStore>();
    public DbSet<ApprovalStepStore> ApprovalSteps => this.Set<ApprovalStepStore>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
