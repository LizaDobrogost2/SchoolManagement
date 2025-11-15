namespace SchoolManagement.Services;

/// <summary>
/// Represents the status of a service operation result.
/// Maps to HTTP status codes for consistent API responses.
/// </summary>
public enum ServiceResultStatus
{
    /// <summary>
    /// Operation completed successfully (HTTP 200 OK).
    /// </summary>
    Ok,

    /// <summary>
    /// Resource created successfully (HTTP 201 Created).
    /// </summary>
    Created,

    /// <summary>
    /// Bad request - validation failed or invalid input (HTTP 400 Bad Request).
    /// </summary>
    BadRequest,

    /// <summary>
    /// Resource not found (HTTP 404 Not Found).
    /// </summary>
    NotFound,

    /// <summary>
    /// Conflict - duplicate resource or state conflict (HTTP 409 Conflict).
    /// </summary>
    Conflict
}
