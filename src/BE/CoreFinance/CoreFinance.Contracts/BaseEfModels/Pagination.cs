namespace CoreFinance.Contracts.BaseEfModels;

public class Pagination
{
    public Pagination()
    {
        PageIndex = 1;
        PageSize = 10;
        TotalRow = 0;
        PageCount = 0;
    }

    public int TotalRow { get; set; }
    public int PageCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}