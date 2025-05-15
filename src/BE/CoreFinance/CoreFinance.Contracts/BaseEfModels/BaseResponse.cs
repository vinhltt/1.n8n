namespace CoreFinance.Contracts.BaseEfModels;

public class BaseResponse<T>
{
    public BaseResponse()
    {
        Body = default;
    }
    public BaseResponse(int statusCode, string message, T? body = default)
    {
        StatusCode = statusCode;
        Message = message;
        Body = body;
    }
    public int? StatusCode { get; set; }
    public T? Body { get; set; }
    public string? Message { get; set; }
}