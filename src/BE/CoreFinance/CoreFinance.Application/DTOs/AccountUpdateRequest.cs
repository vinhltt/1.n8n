using CoreFinance.Contracts.DTOs;

namespace CoreFinance.Application.DTOs;

public class AccountUpdateRequest : BaseUpdateRequest<Guid>
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? CardNumber { get; set; }
    public string? Currency { get; set; }
    public decimal? AvailableLimit { get; set; }
}