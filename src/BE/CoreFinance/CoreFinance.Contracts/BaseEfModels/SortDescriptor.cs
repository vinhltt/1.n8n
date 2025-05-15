using CoreFinance.Contracts.Enums;

namespace CoreFinance.Contracts.BaseEfModels;

public class SortDescriptor
{
    public string? Field { get; set; }
    public SortDirection Direction { get; set; }
}