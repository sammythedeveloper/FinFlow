using FinancialTracker.API.DTOs;
using FinancialTracker.API.Models;

namespace FinancialTracker.API.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterDto request);
    Task<string?> LoginAsync(UserLoginDto request); // Returns a JWT token on success
}