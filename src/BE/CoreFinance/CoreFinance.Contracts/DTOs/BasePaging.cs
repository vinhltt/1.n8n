using CoreFinance.Contracts.BaseEfModels;

namespace CoreFinance.Contracts.DTOs;

public class BasePaging<T> : IBasePaging<T>
{
    public Pagination Pagination { get; set; } = new();
    public IEnumerable<T>? Data { get; set; }
}

public interface IBasePaging<T>
{
    public Pagination Pagination { get; set; }
    public IEnumerable<T>? Data { get; set; }
}