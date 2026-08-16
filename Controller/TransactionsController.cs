using System.Security.Claims;
using FinancialTracker.API.Data;
using FinancialTracker.API.DTOs;
using FinancialTracker.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    // Helper method to extract UserId from JWT claims
    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim != null && int.TryParse(claim.Value, out int id) ? id : 0;
    }

    // GET: api/transactions (Only returns transactions for the logged-in user)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetTransactions()
    {
        var userId = GetUserId();

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                UserId = t.UserId,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    // POST: api/transactions
    [HttpPost]
    public async Task<ActionResult<TransactionResponseDto>> CreateTransaction(TransactionCreateDto request)
    {
        var userId = GetUserId();

        // Verify category exists and belongs to the logged-in user
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);

        if (category == null)
        {
            return BadRequest(new { message = "Category not found or does not belong to you." });
        }

        var transaction = new Transaction
        {
            Amount = request.Amount,
            Description = request.Description,
            Date = request.Date,
            UserId = userId, // Securely assigned from token
            CategoryId = request.CategoryId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var response = new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Date = transaction.Date,
            UserId = transaction.UserId,
            CategoryId = transaction.CategoryId,
            CategoryName = category.Name
        };

        return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Id }, response);
    }
    
    // GET: api/transactions/summary
    [HttpGet("summary")]
    public async Task<ActionResult<TransactionSummaryDto>> GetSummary()
    {
        var userId = GetUserId();

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .ToListAsync();

        decimal totalIncome = transactions
            .Where(t => t.Category?.Type == "Income")
            .Sum(t => t.Amount);

        decimal totalExpenses = transactions
            .Where(t => t.Category?.Type == "Expense")
            .Sum(t => t.Amount);

        var categoryBreakdown = transactions
            .Where(t => t.Category != null)
            .GroupBy(t => new { t.Category!.Name, t.Category.Type })
            .Select(g => new CategorySummaryDto
            {
                CategoryName = g.Key.Name,
                Type = g.Key.Type,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .ToList();

        var summary = new TransactionSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetBalance = totalIncome - totalExpenses,
            CategoryBreakdown = categoryBreakdown
        };

        return Ok(summary);
    }
}