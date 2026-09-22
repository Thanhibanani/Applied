using System.ComponentModel.DataAnnotations;

namespace Applied.Api.Dtos;

public record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(8)] string Password,
    [property: Required] string FullName
);

public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password
);

public record AuthResponse(string AccessToken, DateTime ExpiresAt, string FullName, string Email);
