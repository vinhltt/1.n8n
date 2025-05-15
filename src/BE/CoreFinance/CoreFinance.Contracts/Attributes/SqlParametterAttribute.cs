using System.Data;
using System.Runtime.CompilerServices;

namespace CoreFinance.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SqlParameterAttribute(DbType type, [CallerMemberName] string name = "") : Attribute
{
    public DbType DbType { get; set; } = type;
    private string Name { get; set; } = name;
    public string ParameterName => "@" + Name;
}