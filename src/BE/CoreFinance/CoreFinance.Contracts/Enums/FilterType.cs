namespace CoreFinance.Contracts.Enums;

public enum FilterType
{
    Equal,
    NotEqual,
    StartsWith,
    EndsWith,
    GreaterThan,
    LessThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
    Between,
    NotBetween,
    IsNotNull,
    IsNull,
    IsNotNullOrWhiteSpace,
    IsNullOrWhiteSpace,
    IsEmpty,
    IsNotEmpty,
    In,
    NotIn,
    Contains,
    NotContains
}