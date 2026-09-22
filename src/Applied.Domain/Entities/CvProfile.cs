namespace Applied.Domain.Entities;

/// <summary>
/// The parsed/summarised CV data Applied uses to match and draft
/// applications. One per user.
/// </summary>
public class CvProfile
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public string Tagline { get; set; } = string.Empty;
    public string RawCvText { get; set; } = string.Empty;

    /// <summary>Comma-separated skills/keywords, e.g. "Azure,ServiceNow,C#".</summary>
    public string Skills { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
