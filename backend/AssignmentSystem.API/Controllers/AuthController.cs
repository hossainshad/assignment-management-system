using AssignmentSystem.Core.DTOs;
using AssignmentSystem.Infrastructure.Data;
using AssignmentSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

   
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
       
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

      
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password" });

        // Generate JWT token
        var token = _tokenService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }

    // GET /api/auth/me
    [HttpGet("me")]
    [Authorize] // Must have a valid JWT token
    public async Task<IActionResult> GetCurrentUser()
    {
        // Read the user ID from the JWT token claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null) return Unauthorized();

        var user = await _db.Users.FindAsync(Guid.Parse(userId));

        if (user == null) return NotFound();

        return Ok(new UserProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }
}