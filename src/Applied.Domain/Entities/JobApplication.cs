namespace Applied.Domain.Entities;

/// <summary>
/// The user's draft/approved/sent application for one JobListing:
/// the match score, why it matched, the cover-letter draft, and its
/// place in the approval workflow.
/// </summary>
public class JobApplication
{
    public int Id { get; set; }

    public int JobListingId { get; set; }
    public JobListing? JobListing { get; set; }

    public int MatchScore { get; set; }

    /// <summary>Comma-separated short reasons, e.g. "Azure/AZ-900,2. linje support".</summary>
    public string MatchReasons { get; set; } = string.Empty;

    public string CoverLetter { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
}
