using FinancialTracker.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.API.Controller;

[ApiController]
[Route("api/public")]
public class PublicDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public PublicDataController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Type
            })
            .ToListAsync();

        return Ok(categories);
    }

    [AllowAnonymous]
    [HttpGet("transaction-count")]
    public async Task<IActionResult> GetTransactionCount()
    {
        var count = await _context.Transactions.CountAsync();

        return Ok(new
        {
            transactionCount = count
        });
    }
}