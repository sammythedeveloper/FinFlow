using System.ComponentModel.DataAnnotations;

namespace FinancialTracker.API.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required, MaxLength(20)]
    public string Type { get; set; } = "Expense"; // "Income" or "Expense"

    // Foreign Key to User (now Guid)
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Transaction> Transactions { get; set; } = new();
}