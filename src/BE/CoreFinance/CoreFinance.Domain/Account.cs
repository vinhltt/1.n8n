using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreFinance.Contracts.BaseEfModels;

namespace CoreFinance.Domain;

public enum AccountType
{
    Bank,
    Wallet,
    CreditCard,
    DebitCard,
    Cash
}

public class Account : BaseEntity<Guid>
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    public AccountType Type { get; set; }

    [MaxLength(32)]
    public string? CardNumber { get; set; }

    [Required]
    [MaxLength(10)]
    public string? Currency { get; set; }

    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal? AvailableLimit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation property
    [InverseProperty("Account")]
    public ICollection<Transaction>? Transactions { get; set; }
}