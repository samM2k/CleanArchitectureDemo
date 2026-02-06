namespace ExpenseApproval.Domain;

/// <summary>
/// Represents an expense submitted by an employee for approval and reimbursement within the organization.
/// </summary>
public sealed class Expense
{
    private readonly List<ApprovalStep> _approvalSteps = new();
    
    /// <summary>
    /// Gets the unique identifier of the expense.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the unique identifier for the employee who the expense has come from.
    /// </summary>
    public Guid EmployeeId { get; }

    /// <summary>
    /// Gets the amount of the expense. Must be greater than 0.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency of the expense amount, represented as a 3-letter ISO code (e.g., "USD", "EUR").
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Gets the category of the expense, which classifies the type of expense (e.g., Travel, Meals, Office Supplies).
    /// </summary>
    public ExpenseCategory Category { get; }

    /// <summary>
    /// Gets the hash value representing the contents of the receipt.
    /// </summary>
    public string ReceiptHash { get; }

    /// <summary>
    /// Gets the current status of the expense.
    /// </summary>
    public ExpenseStatus Status { get; private set; }

    /// <summary>
    /// Gets the list of approval steps required for this expense, in the order they must be completed.
    /// </summary>
    public IReadOnlyList<ApprovalStep> ApprovalSteps => this._approvalSteps;

    /// <summary>
    /// Initializes a new draft expense for the specified employee with the given amount, currency, category, and
    /// receipt hash.
    /// </summary>
    /// <remarks>The expense is created in the draft status and assigned a new unique identifier. Use this
    /// constructor when submitting a new expense for an employee.</remarks>
    /// <param name="employeeId">The unique identifier of the employee associated with the expense.</param>
    /// <param name="amount">The monetary amount of the expense.</param>
    /// <param name="currency">The ISO currency code representing the currency of the expense.</param>
    /// <param name="category">The category of the expense, indicating its type or purpose.</param>
    /// <param name="receiptHash">The hash value of the receipt associated with the expense. Used to verify receipt authenticity.</param>
    public Expense(Guid employeeId, decimal amount, string currency, ExpenseCategory category, string receiptHash) : this(Guid.NewGuid(), employeeId, amount, currency, category, receiptHash, ExpenseStatus.Draft, new())
    {
    }

    /// <summary>
    /// Initializes a new instance of the Expense class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier for the expense.</param>
    /// <param name="employeeId">The unique identifier of the employee associated with the expense.</param>
    /// <param name="amount">The monetary amount of the expense.</param>
    /// <param name="currency">The ISO currency code representing the currency of the expense.</param>
    /// <param name="category">The category of the expense.</param>
    /// <param name="receiptHash">The hash value of the expense receipt for verification purposes.</param>
    /// <param name="status">The current status of the expense.</param>
    /// <param name="steps">The list of approval steps required for the expense.</param>
    /// <exception cref="DomainException">Thrown if any parameter is invalid, such as being empty, null, or unspecified.</exception>
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

    /// <summary>
    /// Submits the expense for review according to the specified policy, initiating the required approval steps.
    /// </summary>
    /// <remarks>After submission, the expense status changes to <see cref="ExpenseStatus.InReview"/> and approval steps are generated
    /// based on the policy. Ensure that the expense is in the <see cref="ExpenseStatus.Draft"/> status before calling this method.</remarks>
    /// <param name="policy">The expense policy that determines whether the expense can be submitted and defines the required approval roles.</param>
    /// <exception cref="DomainException">Thrown if the expense is not in the <see cref="ExpenseStatus.Draft"/> status or if the policy does not produce any approval steps.</exception>
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

    /// <summary>
    /// Attempts to approve the next pending approval step for the expense using the specified approver and role.
    /// </summary>
    /// <remarks>After approval, the expense status is updated to either <see cref="ExpenseStatus.Approved"/> if all steps are approved,
    /// or <see cref="ExpenseStatus.InReview"/> if further approvals are required.</remarks>
    /// <param name="approverId">The unique identifier of the user performing the approval. This value is used to record who approved the step.</param>
    /// <param name="role">The role of the approver. Must match the role required for the next pending approval step.</param>
    /// <exception cref="DomainException">Thrown if the expense is in a status that cannot be approved, if the expense has not been submitted, if there
    /// are no remaining approval steps, or if the specified role does not match the next required approval step.</exception>
    public void Approve(Guid approverId, ApproverRole role)
    {
        if (this.Status is ExpenseStatus.Rejected or ExpenseStatus.Paid)
            throw new DomainException($"Cannot approve an expense in status {this.Status}.");

        if (this.Status == ExpenseStatus.Draft)
            throw new DomainException("Submit the expense before approving.");

        var next = this._approvalSteps.OrderBy(s => s.Order).FirstOrDefault(s => s.Status == ApprovalStatus.Pending);
        if (next is null)
            throw new DomainException("No remaining approval steps.");

        if (next.Role != role)
            throw new DomainException($"Next required approval is {next.Role}, not {role}.");

        next.MarkApproved(approverId);

        this.Status = this._approvalSteps.All(s => s.IsApproved) ? ExpenseStatus.Approved : ExpenseStatus.InReview;
    }

    /// <summary>
    /// Rejects the expense by marking the next required approval step as <see cref="ApprovalStatus.Rejected"/> and updating the expense status to
    /// <see cref="ExpenseStatus.Rejected"/>.
    /// </summary>
    /// <param name="approverId">The unique identifier of the approver performing the rejection.</param>
    /// <param name="role">The role of the approver responsible for the current approval step.</param>
    /// <param name="reason">The reason for rejecting the expense.</param>
    /// <exception cref="DomainException">Thrown if the rejection reason is missing, if the expense is already approved or paid, if there are no remaining
    /// approval steps, or if the provided role does not match the next required approval step.</exception>
    public void Reject(Guid approverId, ApproverRole role, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Rejection reason is required.");
        if (this.Status is ExpenseStatus.Approved or ExpenseStatus.Paid)
            throw new DomainException($"Cannot reject an expense in status {this.Status}.");

        var next = this._approvalSteps.OrderBy(s => s.Order).FirstOrDefault(s => s.Status == ApprovalStatus.Pending);
        if (next is null)
            throw new DomainException("No remaining approval steps.");

        if (next.Role != role)
            throw new DomainException($"Next required approval is {next.Role}, not {role}.");

        next.MarkRejected(approverId, reason);
        this.Status = ExpenseStatus.Rejected;
    }

    /// <summary>
    /// Marks the expense as paid if its current status is Approved.
    /// </summary>
    /// <remarks>Use this method to transition an expense from <see cref="ExpenseStatus.Approved"/> to <see cref="ExpenseStatus.Paid"/>. This operation is only valid
    /// when the expense has been approved; attempting to mark an expense in any other state as paid will result in an
    /// exception.</remarks>
    /// <exception cref="DomainException">Thrown if the expense status is not Approved.</exception>
    public void MarkPaid()
    {
        if (this.Status != ExpenseStatus.Approved)
            throw new DomainException("Only Approved expenses can be marked as Paid.");
        this.Status = ExpenseStatus.Paid;
    }
}
