namespace CoreFinance.Domain.Enums;

/// <summary>
/// Represents the type of financial account
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Bank account
    /// </summary>
    Bank,
    
    /// <summary>
    /// Digital wallet
    /// </summary>
    Wallet,
    
    /// <summary>
    /// Credit card account
    /// </summary>
    CreditCard,
    
    /// <summary>
    /// Debit card account
    /// </summary>
    DebitCard,
    
    /// <summary>
    /// Cash account
    /// </summary>
    Cash
} 