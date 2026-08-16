using System.ComponentModel.DataAnnotations;

namespace FinancialTracker.API.DTOs;

public class CategoryCreateDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required, MaxLength(20)]
    public string Type { get; set; } = "Expense"; // "Income" or "Expense"
}