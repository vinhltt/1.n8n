namespace CoreFinance.Contracts.BaseEfModels;

public class GlobalVariable<T>
{
    private static T? _object;

    public GlobalVariable(T? @object)
    {
        _object = @object;
    }

    public static T? Object => _object;
}