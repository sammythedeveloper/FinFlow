using System.ComponentModel.DataAnnotations;

namespace FinancialTracker.API.Models;

public class User
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property for relationships
    public List<Transaction> Transactions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
}