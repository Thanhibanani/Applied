using System.Security.Claims;
using Applied.Api.Dtos;
using Applied.Domain.Entities;
using Applied.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applied.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly AppliedDbContext _db;

    public JobsController(AppliedDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException("No user id on the authenticated principal.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetAll()
    {
        var jobs = await _db.JobListings
            .Where(j => j.UserId == UserId)
            .Include(j => j.Application)
            .OrderByDescending(j => j.AddedAt)
            .ToListAsync();

        return Ok(jobs.Select(ToResponse));
    }

    /// <summary>
    /// Registers a job the user found and pasted in themselves (a URL from
    /// Finn.no/LinkedIn/elsewhere). There is deliberately no endpoint that
    /// scans a platform on its own — see README "Why no auto-scanning".
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<JobResponse>> Create(CreateJobRequest request)
    {
        var job = new JobListing
        {
            UserId = UserId,
            Title = request.Title,
            Company = request.Company,
            Location = request.Location,
            Source = request.Source,
            Url = request.Url,
            Deadline = request.Deadline
        };

        _db.JobListings.Add(job);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = job.Id }, ToResponse(job));
    }

    [HttpPut("{jobId:int}/application")]
    public async Task<ActionResult<ApplicationResponse>> UpsertApplication(int jobId, UpdateApplicationRequest request)
    {
        var job = await _db.JobListings
            .Include(j => j.Application)
            .FirstOrDefaultAsync(j => j.Id == jobId && j.UserId == UserId);
        if (job is null) return NotFound();

        var application = job.Application;
        if (application is null)
        {
            application = new JobApplication { JobListingId = job.Id };
            _db.JobApplications.Add(application);
        }

        application.MatchScore = request.MatchScore;
        application.MatchReasons = string.Join(",", request.MatchReasons);
        application.CoverLetter = request.CoverLetter;
        application.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ToApplicationResponse(application));
    }

    /// <summary>
    /// The only place an application's status can change. Sent can only be
    /// reached from Approved — never directly from New — so nothing can
    /// mark itself "sent" without a human approving it first.
    /// </summary>
    [HttpPost("{jobId:int}/application/status")]
    public async Task<ActionResult<ApplicationResponse>> SetStatus(int jobId, ApplicationStatusRequest request)
    {
        var application = await _db.JobApplications
            .Include(a => a.JobListing)
            .FirstOrDefaultAsync(a => a.JobListingId == jobId && a.JobListing!.UserId == UserId);
        if (application is null) return NotFound();

        if (request.Status == ApplicationStatus.Sent && application.Status != ApplicationStatus.Approved)
            return BadRequest(new { message = "En søknad kan kun markeres som sendt etter at den er godkjent." });

        application.Status = request.Status;
        application.UpdatedAt = DateTime.UtcNow;
        if (request.Status == ApplicationStatus.Sent)
            application.SentAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ToApplicationResponse(application));
    }

    private static JobResponse ToResponse(JobListing job) => new(
        job.Id, job.Title, job.Company, job.Location, job.Source.ToString(), job.Url, job.Deadline, job.AddedAt,
        job.Application is null ? null : ToApplicationResponse(job.Application));

    private static ApplicationResponse ToApplicationResponse(JobApplication a) => new(
        a.Id, a.MatchScore,
        string.IsNullOrEmpty(a.MatchReasons) ? Array.Empty<string>() : a.MatchReasons.Split(',', StringSplitOptions.RemoveEmptyEntries),
        a.CoverLetter, a.Status.ToString(), a.UpdatedAt, a.SentAt);
}
