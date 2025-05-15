using CoreFinance.Contracts.DTOs;

namespace CoreFinance.Application.DTOs;

public class AccountUpdateRequest : BaseUpdateRequest<Guid>
{
    public string? AccountName { get; set; }
    public string? AccountType { get; set; }
    public string? CardNumber { get; set; }
    public string? Currency { get; set; }
    public decimal? AvailableLimit { get; set; }
}