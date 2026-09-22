using Applied.Domain.Entities;

namespace Applied.Api.Dtos;

public record CreateJobRequest(string Title, string Company, string Location, JobSource Source, string Url, string? Deadline);

public record UpdateApplicationRequest(int MatchScore, string[] MatchReasons, string CoverLetter);

public record ApplicationStatusRequest(ApplicationStatus Status);

public record JobResponse(
    int Id,
    string Title,
    string Company,
    string Location,
    string Source,
    string Url,
    string? Deadline,
    DateTime AddedAt,
    ApplicationResponse? Application
);

public record ApplicationResponse(
    int Id,
    int MatchScore,
    string[] MatchReasons,
    string CoverLetter,
    string Status,
    DateTime UpdatedAt,
    DateTime? SentAt
);
