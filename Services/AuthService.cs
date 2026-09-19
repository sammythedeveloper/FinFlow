using System.Security.Claims;
using FinancialTracker.API.Data;
using FinancialTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> EnsureUserExistsAsync(ClaimsPrincipal principal)
    {
        var supabaseUserId = principal.FindFirstValue("sub")
                          ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(supabaseUserId) ||
            !Guid.TryParse(supabaseUserId, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid or missing user ID in token."
            );
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email)
                     ?? principal.FindFirstValue("email")
                     ?? string.Empty;

            var username = principal.FindFirstValue(ClaimTypes.Name)
                         ?? email.Split('@')[0];

            user = new User
            {
                Id = userId,
                Email = email,
                Username = username,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("===== USER CREATION FAILED =====");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("================================");

                throw;
            }
        }

        return user;
    }
}