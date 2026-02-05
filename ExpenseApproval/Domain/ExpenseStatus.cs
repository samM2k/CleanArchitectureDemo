namespace ExpenseApproval.Domain;

public enum ExpenseStatus 
{
    Unspecified = 0,
    Draft = 1, 
    Submitted = 2, 
    InReview = 3, 
    Rejected = 4, 
    Approved = 5, 
    Paid = 6 
}
