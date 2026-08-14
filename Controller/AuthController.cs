using FinancialTracker.API.DTOs;
using FinancialTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user == null)
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        return Ok(new { message = "User registered successfully!", username = user.Username });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        var token = await _authService.LoginAsync(request);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(new { token = token });
    }
}