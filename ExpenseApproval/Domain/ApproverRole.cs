namespace ExpenseApproval.Domain;

/// <summary>
/// Specifies the role of an approver in a financial or organizational approval workflow.
/// </summary>
public enum ApproverRole 
{
    /// <summary>
    /// The default nullish value, indicating that the approver role has not been specified. This value should not be used in practice and serves as a placeholder for uninitialized states.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Represents a manager role within the organization.
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Represents a finance role within the organization.
    /// </summary>
    Finance = 2, 

    /// <summary>
    /// Represents the Chief Financial Officer role within the organization.
    /// </summary>
    CFO = 3
}
