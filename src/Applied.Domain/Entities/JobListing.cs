namespace Applied.Domain.Entities;

public enum JobSource
{
    Finn = 0,
    LinkedIn = 1,
    Other = 2
}

/// <summary>
/// A job posting the user has added (by pasting a link) or Applied has
/// found. Ingestion is always "fetch and parse one URL at a time" —
/// there is no background scraper, by design (see README).
/// </summary>
public class JobListing
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobSource Source { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Deadline { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public JobApplication? Application { get; set; }
}
