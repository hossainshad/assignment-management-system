using AssignmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectController : ControllerBase
{
    private readonly AppDbContext _db;
    public SubjectController(AppDbContext db) => _db = db;

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/subject  → subjects for the logged-in teacher
    [HttpGet]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMySubjects()
    {
        var subjects = await _db.Subjects
            .Where(s => s.TeacherId == GetUserId())
            .Select(s => new { s.Id, s.Name, s.Code, s.ClassId })
            .ToListAsync();

        return Ok(subjects);
    }
}
