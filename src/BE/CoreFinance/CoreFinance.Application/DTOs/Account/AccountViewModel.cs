using CoreFinance.Contracts.DTOs;
using CoreFinance.Domain.Enums;

namespace CoreFinance.Application.DTOs.Account;

public class AccountViewModel : BaseViewModel<Guid>
{
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public AccountType Type { get; set; }
    public string? CardNumber { get; set; }
    public string? Currency { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal? AvailableLimit { get; set; }
}