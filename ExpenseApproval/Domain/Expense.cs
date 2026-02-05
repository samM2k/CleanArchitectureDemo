namespace ExpenseApproval.Domain;

public sealed class Expense
{
    public Guid Id { get; }
    public Guid EmployeeId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public ExpenseCategory Category { get; }
    public string ReceiptHash { get; }

    public ExpenseStatus Status { get; private set; }

    private readonly List<ApprovalStep> _approvalSteps = new();
    public IReadOnlyList<ApprovalStep> ApprovalSteps => this._approvalSteps;

    public Expense(Guid employeeId, decimal amount, string currency, ExpenseCategory category, string receiptHash) : this(Guid.NewGuid(), employeeId, amount, currency, category, receiptHash, ExpenseStatus.Draft, new())
    {
    }

    public Expense(Guid id, Guid employeeId, decimal amount, string currency, ExpenseCategory category, string receiptHash, ExpenseStatus status, List<ApprovalStep> steps)
    {
        if (id == Guid.Empty) throw new DomainException("Id is required.");
        if (employeeId == Guid.Empty) throw new DomainException("EmployeeId is required.");
        if (amount <= 0) throw new DomainException("Amount must be greater than 0.");
        if (category == ExpenseCategory.Unspecified) throw new DomainException("Category is required.");
        if(status == ExpenseStatus.Unspecified) throw new DomainException("Status is required.");
        if (steps == null)
            throw new DomainException("Steps list is required.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");
        if (string.IsNullOrWhiteSpace(receiptHash)) throw new DomainException("Receipt hash is required.");

        this.Id = id;
        this.EmployeeId = employeeId;
        this.Amount = amount;
        this.Currency = currency.Trim().ToUpperInvariant();
        this.Category = category;
        this.ReceiptHash = receiptHash.Trim();
        this.Status = status;
        this._approvalSteps = steps;
    }

    public void Submit(IExpensePolicy policy)
    {
        if (this.Status != ExpenseStatus.Draft)
            throw new DomainException($"Only Draft expenses can be submitted (currently {this.Status}).");

        policy.EnsureExpenseAllowed(this);

        var roles = policy.RequiredApprovals(this);
        if (roles.Count == 0)
            throw new DomainException("Policy produced no approval steps.");

        this._approvalSteps.Clear();
        var order = 1;
        foreach (var role in roles)
            this._approvalSteps.Add(new ApprovalStep(role, order++));

        this.Status = ExpenseStatus.InReview;
    }

    public void Approve(Guid approverId, ApproverRole role)
    {
        if (this.Status is ExpenseStatus.Rejected or ExpenseStatus.Paid)
            throw new DomainException($"Cannot approve an expense in status {this.Status}.");

        if (this.Status == ExpenseStatus.Draft)
            throw new DomainException("Submit the expense before approving.");

        var next = this._approvalSteps.OrderBy(s => s.Order).FirstOrDefault(s => !s.IsApproved && !s.IsRejected);
        if (next is null)
            throw new DomainException("No remaining approval steps.");

        if (next.Role != role)
            throw new DomainException($"Next required approval is {next.Role}, not {role}.");

        next.MarkApproved(approverId);

        this.Status = this._approvalSteps.All(s => s.IsApproved) ? ExpenseStatus.Approved : ExpenseStatus.InReview;
    }

    public void Reject(Guid approverId, ApproverRole role, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Rejection reason is required.");
        if (this.Status is ExpenseStatus.Approved or ExpenseStatus.Paid)
            throw new DomainException($"Cannot reject an expense in status {this.Status}.");

        var next = this._approvalSteps.OrderBy(s => s.Order).FirstOrDefault(s => !s.IsApproved && !s.IsRejected);
        if (next is null)
            throw new DomainException("No remaining approval steps.");

        if (next.Role != role)
            throw new DomainException($"Next required approval is {next.Role}, not {role}.");

        next.MarkRejected(approverId, reason);
        this.Status = ExpenseStatus.Rejected;
    }

    public void MarkPaid()
    {
        if (this.Status != ExpenseStatus.Approved)
            throw new DomainException("Only Approved expenses can be marked as Paid.");
        this.Status = ExpenseStatus.Paid;
    }
}
