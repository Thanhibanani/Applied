using Microsoft.AspNetCore.Identity;

namespace Applied.Domain.Entities;

/// <summary>
/// A registered person using Applied. Extends the built-in Identity user
/// with the profile fields Applied actually needs.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CvProfile? CvProfile { get; set; }
    public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
}
