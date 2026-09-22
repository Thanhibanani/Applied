using System.Security.Claims;
using Applied.Domain.Entities;
using Applied.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applied.Api.Controllers;

public record UpdateProfileRequest(string Tagline, string RawCvText, string Skills, string Location);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppliedDbContext _db;

    public ProfileController(AppliedDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException("No user id on the authenticated principal.");

    [HttpGet]
    public async Task<ActionResult<CvProfile>> Get()
    {
        var profile = await _db.CvProfiles.FirstOrDefaultAsync(p => p.UserId == UserId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<CvProfile>> Upsert(UpdateProfileRequest request)
    {
        var profile = await _db.CvProfiles.FirstOrDefaultAsync(p => p.UserId == UserId);
        if (profile is null)
        {
            profile = new CvProfile { UserId = UserId };
            _db.CvProfiles.Add(profile);
        }

        profile.Tagline = request.Tagline;
        profile.RawCvText = request.RawCvText;
        profile.Skills = request.Skills;
        profile.Location = request.Location;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(profile);
    }
}
