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
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var categories = await _context.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                UserId = c.UserId
            })
            .ToListAsync();

        return Ok(categories);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryCreateDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var category = new Category
        {
            Name = request.Name,
            Type = request.Type,
            UserId = userId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var response = new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            UserId = category.UserId
        };

        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, response);
    }
}