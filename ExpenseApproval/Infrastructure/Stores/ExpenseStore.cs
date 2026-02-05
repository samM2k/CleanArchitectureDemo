namespace ExpenseApproval.Infrastructure.Stores;

public sealed class ExpenseStore
{
    public Guid ExpenseId { get; set; }
    public Guid EmployeeId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "AUD";

    public string Category { get; set; } = "";   // store enums as string (or int)
    public string Status { get; set; } = "";

    public string ReceiptHash { get; set; } = "";

    public List<ApprovalStepStore> ApprovalSteps { get; set; } = new();
}
