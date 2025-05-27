using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Domain;

public class Account : BaseEntity<Guid>
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public AccountType Type { get; set; }

    [MaxLength(32)]
    public string? CardNumber { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal InitialBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? AvailableLimit { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
    
    [Required]
    public bool IsActive { get; set; }

    // Navigation property
    [InverseProperty("Account")]
    public ICollection<Transaction>? Transactions { get; set; }
}