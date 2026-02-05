namespace ExpenseApproval.Infrastructure.Stores;

public sealed class ApprovalStepStore
{
    public Guid Id { get; set; }
    public Guid ExpenseId { get; set; }

    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Order { get; set; }


    public Guid? ActionedBy { get; set; }
    public DateTimeOffset? ActionedAt { get; set; }
    public string? RejectionReason { get; set; }
}
