using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreFinance.Contracts.BaseEfModels;

[Microsoft.EntityFrameworkCore.Index(nameof(Id))]
public abstract class BaseEntity<TKey>
{
    public virtual BaseEntity<TKey> SetDefaultValue(string createBy)
    {
        CreateAt = DateTime.Now;
        UpdateAt = CreateAt;
        CreateBy = createBy;
        return this;
    }

    public virtual BaseEntity<TKey> SetValueUpdate(string updateBy)
    {
        UpdateAt = DateTime.Now;
        UpdateBy = updateBy;
        return this;
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public virtual TKey? Id { get; set; }

    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public string? Deleted { get; set; }
}