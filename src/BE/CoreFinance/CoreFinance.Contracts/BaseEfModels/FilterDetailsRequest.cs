using CoreFinance.Contracts.Enums;

namespace CoreFinance.Contracts.BaseEfModels;

public class FilterDetailsRequest
{
    public string? AttributeName { get; set; }
    public string? Value { get; set; }
    public FilterType FilterType { get; set; }
}