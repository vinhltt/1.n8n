namespace CoreFinance.Domain.Enums;

/// <summary>
/// Represents the status of an expected transaction
/// </summary>
public enum ExpectedTransactionStatus
{
    /// <summary>
    /// Transaction is pending execution
    /// </summary>
    Pending,
    
    /// <summary>
    /// Transaction has been confirmed
    /// </summary>
    Confirmed,
    
    /// <summary>
    /// Transaction has been cancelled
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Transaction has been completed
    /// </summary>
    Completed
} 