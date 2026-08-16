namespace FinancialTracker.API.DTOs;

public class TransactionSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetBalance { get; set; }
    public List<CategorySummaryDto> CategoryBreakdown { get; set; } = new();
}

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}