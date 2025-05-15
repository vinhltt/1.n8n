namespace CoreFinance.Contracts.BaseEfModels;
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
public class FilterBodyRequest : BodyRequest, IFilterBodyRequest
{
    public string? LangId { get; set; }
    public string? SearchValue { get; set; }
    public FilterRequest? Filter { get; set; }
    public IEnumerable<SortDescriptor>? Orders { get; set; }
    public Pagination? Pagination { get; set; }
}
public interface IFilterBodyRequest : IBodyRequest
{
    public string? LangId { get; set; }
    public string? SearchValue { get; set; }
    public FilterRequest? Filter { get; set; }
    public IEnumerable<SortDescriptor>? Orders { get; set; }
    public Pagination? Pagination { get; set; }
}
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).