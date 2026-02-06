namespace ExpenseApproval.Domain;

/// <summary>
/// Specifies the category of an expense for classification and reporting purposes.
/// </summary>
public enum ExpenseCategory 
{
    /// <summary>
    /// The default nullish value, indicating that the expense category has not been specified.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Represents the travel expense category.
    /// </summary>
    Travel = 1, 

    /// <summary>
    /// Represents the meals expense category.
    /// </summary>
    Meals = 2, 

    /// <summary>
    /// Represents the office supplies expense category.
    /// </summary>
    OfficeSupplies = 3,

    /// <summary>
    /// Represents the alcohol expense category.
    /// </summary>
    Alcohol = 4, 

    /// <summary>
    /// Represents expenses that don't fall under a specific existing category.
    /// </summary>
    Other = 5
}
