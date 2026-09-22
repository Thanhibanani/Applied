namespace Applied.Domain.Entities;

/// <summary>
/// Where an application is in the human-approval workflow. Applied never
/// moves a job past Approved on its own — Sent is only ever set after
/// the user has confirmed the send, in chat or in the app.
/// </summary>
public enum ApplicationStatus
{
    New = 0,
    Approved = 1,
    Rejected = 2,
    Sent = 3
}
