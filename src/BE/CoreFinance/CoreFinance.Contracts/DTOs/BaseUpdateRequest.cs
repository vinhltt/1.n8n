namespace CoreFinance.Contracts.DTOs;

public abstract class BaseUpdateRequest<TKey> : BaseDto
{
    public virtual TKey? Id { get; set; }
    public bool? Deleted { get; set; }
}