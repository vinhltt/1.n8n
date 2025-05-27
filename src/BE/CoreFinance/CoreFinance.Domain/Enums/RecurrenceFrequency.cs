namespace CoreFinance.Domain.Enums;

/// <summary>
/// Represents the frequency of recurring transactions
/// </summary>
public enum RecurrenceFrequency
{
    /// <summary>
    /// Custom frequency (use CustomIntervalDays)
    /// </summary>
    Custom = 0,
    
    /// <summary>
    /// Daily frequency (every day)
    /// </summary>
    Daily = 1,
    
    /// <summary>
    /// Weekly frequency (every 7 days)
    /// </summary>
    Weekly = 7,
    
    /// <summary>
    /// Biweekly frequency (every 14 days)
    /// </summary>
    Biweekly = 14,
    
    /// <summary>
    /// Monthly frequency (every 30 days)
    /// </summary>
    Monthly = 30,
    
    /// <summary>
    /// Quarterly frequency (every 90 days)
    /// </summary>
    Quarterly = 90,
    
    /// <summary>
    /// Semi-annually frequency (every 180 days)
    /// </summary>
    SemiAnnually = 180,
    
    /// <summary>
    /// Annually frequency (every 365 days)
    /// </summary>
    Annually = 365
} 