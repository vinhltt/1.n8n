namespace CoreFinance.Domain.Enums;

/// <summary>
/// Represents the category type of a transaction
/// </summary>
public enum CategoryType
{
    /// <summary>
    /// Income transaction
    /// </summary>
    Income,
    
    /// <summary>
    /// Expense transaction
    /// </summary>
    Expense,
    
    /// <summary>
    /// Transfer transaction
    /// </summary>
    Transfer,
    
    /// <summary>
    /// Fee transaction
    /// </summary>
    Fee,
    
    /// <summary>
    /// Other type of transaction
    /// </summary>
    Other
} 