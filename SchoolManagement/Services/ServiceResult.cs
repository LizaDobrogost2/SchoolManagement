namespace SchoolManagement.Services;

/// <summary>
/// Represents the result of a service operation.
/// Provides a consistent way to return success/failure information along with data or error messages.
/// </summary>
/// <typeparam name="T">The type of data returned on success.</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the data returned by the operation (only populated on success).
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets the error message (only populated on failure).
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the status of the operation result.
    /// </summary>
    public ServiceResultStatus Status { get; init; }

    /// <summary>
    /// Creates a successful result with data and 200 OK status.
    /// </summary>
    /// <param name="data">The data to return.</param>
    /// <returns>A successful service result.</returns>
    public static ServiceResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data,
        Status = ServiceResultStatus.Ok
    };

    /// <summary>
    /// Creates a successful result with data and 201 Created status.
    /// Use this for POST operations that create new resources.
    /// </summary>
    /// <param name="data">The created data to return.</param>
    /// <returns>A successful service result with Created status.</returns>
    public static ServiceResult<T> Created(T data) => new()
    {
        IsSuccess = true,
        Data = data,
        Status = ServiceResultStatus.Created
    };

    /// <summary>
    /// Creates a failed result with 404 Not Found status.
    /// Use this when a requested resource doesn't exist.
    /// </summary>
    /// <param name="message">The error message describing what was not found.</param>
    /// <returns>A failed service result with NotFound status.</returns>
    public static ServiceResult<T> NotFound(string message) => new()
    {
        IsSuccess = false,
        ErrorMessage = message,
        Status = ServiceResultStatus.NotFound
    };

    /// <summary>
    /// Creates a failed result with 400 Bad Request status.
    /// Use this for validation errors or invalid input.
    /// </summary>
    /// <param name="message">The error message describing the validation issue.</param>
    /// <returns>A failed service result with BadRequest status.</returns>
    public static ServiceResult<T> BadRequest(string message) => new()
    {
        IsSuccess = false,
        ErrorMessage = message,
        Status = ServiceResultStatus.BadRequest
    };

    /// <summary>
    /// Creates a failed result with 409 Conflict status.
    /// Use this for duplicate resources or state conflicts.
    /// </summary>
    /// <param name="message">The error message describing the conflict.</param>
    /// <returns>A failed service result with Conflict status.</returns>
    public static ServiceResult<T> Conflict(string message) => new()
    {
        IsSuccess = false,
        ErrorMessage = message,
        Status = ServiceResultStatus.Conflict
    };
}
