using Applied.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Applied.Infrastructure;

public class AppliedDbContext : IdentityDbContext<ApplicationUser>
{
    public AppliedDbContext(DbContextOptions<AppliedDbContext> options) : base(options)
    {
    }

    public DbSet<CvProfile> CvProfiles => Set<CvProfile>();
    public DbSet<JobListing> JobListings => Set<JobListing>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CvProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.CvProfile)
            .HasForeignKey<CvProfile>(p => p.UserId);

        builder.Entity<JobListing>()
            .HasOne(j => j.User)
            .WithMany(u => u.JobListings)
            .HasForeignKey(j => j.UserId);

        builder.Entity<JobApplication>()
            .HasOne(a => a.JobListing)
            .WithOne(j => j.Application)
            .HasForeignKey<JobApplication>(a => a.JobListingId);
    }
}
