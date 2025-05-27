namespace CoreFinance.Application.DTOs.ExpectedTransaction;

public class AdjustTransactionRequest
{
    public decimal NewAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
}