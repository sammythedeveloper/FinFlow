using System.ComponentModel.DataAnnotations;

namespace FinancialTracker.API.DTOs;

public class TransactionCreateDto
{
    [Required]
    public decimal Amount { get; set; }
    
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    public int CategoryId { get; set; }
}