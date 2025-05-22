namespace CoreFinance.Contracts.DTOs;

public abstract class BaseViewModel<TKey> : BaseDto
{
    public virtual TKey? Id { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public string? Deleted { get; set; }
}