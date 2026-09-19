using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialTracker.API.Models;

public class Transaction
{
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime Date { get; set; } = DateTime.UtcNow;

    // Foreign Key to User (now Guid to match Supabase)
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Foreign Key to Category
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}