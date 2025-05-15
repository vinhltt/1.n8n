namespace CoreFinance.Contracts.DTOs;

public interface IBaseTreeViewModel<T>
{
    public T Data { get; set; }
    public List<IBaseTreeViewModel<T>> Children { get; set; }
}