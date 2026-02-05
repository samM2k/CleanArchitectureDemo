namespace ExpenseApproval.Domain;

public sealed class ApprovalStep
{
    public Guid Id { get; }
    public ApproverRole Role { get; }
    public int Order { get; }

    public ApprovalStatus Status { get; private set; }
    public Guid? ActionedBy { get; private set; }
    public DateTimeOffset? ActionedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public bool IsApproved => this.Status == ApprovalStatus.Approved;
    public bool IsRejected => this.Status == ApprovalStatus.Rejected;

    public ApprovalStep(Guid id, ApproverRole role, int order, ApprovalStatus status, Guid? actionedBy, DateTimeOffset? actionedAt, string? rejectionReason)
    {
        if (id == Guid.Empty) throw new DomainException("Id is required.");
        if (role == ApproverRole.Unspecified) throw new DomainException("Role is required.");

        this.Id = id;
        this.Role = role;
        this.Order = order;
        this.Status = status;
        this.ActionedBy = actionedBy;
        this.ActionedAt = actionedAt;
        this.RejectionReason = rejectionReason;
    }

    public ApprovalStep(ApproverRole role, int order) : this(Guid.NewGuid(), role, order, ApprovalStatus.Pending, null, null, null)
    {
    }

    public void MarkApproved(Guid approverId)
    {
        if (approverId == Guid.Empty) throw new DomainException("ApproverId is required.");
        if (this.Status != ApprovalStatus.Pending) throw new DomainException("Step already actioned.");

        this.Status = ApprovalStatus.Approved;
        this.ActionedBy = approverId;
        this.ActionedAt = DateTimeOffset.UtcNow;
    }

    public void MarkRejected(Guid approverId, string reason)
    {
        if (approverId == Guid.Empty) throw new DomainException("ApproverId is required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Reason is required.");
        if (this.Status != ApprovalStatus.Pending) throw new DomainException("Step already actioned.");

        this.Status = ApprovalStatus.Rejected;
        this.RejectionReason = reason.Trim();
        this.ActionedBy = approverId;
        this.ActionedAt = DateTimeOffset.UtcNow;
    }
}
