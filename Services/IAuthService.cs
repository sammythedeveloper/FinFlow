using FinancialTracker.API.Models;
using System.Security.Claims;

namespace FinancialTracker.API.Services;

public interface IAuthService
{
    Task<User> EnsureUserExistsAsync(ClaimsPrincipal principal);
}