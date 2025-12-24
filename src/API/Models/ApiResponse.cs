namespace API.Models;

/// <summary>
/// Standard API response wrapper for consistent response format across all endpoints.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// A message describing the result of the operation.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The response data (null if the request failed).
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// List of validation errors or additional error details (null if no errors).
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Timestamp of when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a successful response without data.
    /// </summary>
    public static ApiResponse<object> SuccessResponse(string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse<object>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = null,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Non-generic API response for operations that don't return data.
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response without data.
    /// </summary>
    public static new ApiResponse SuccessResponse(string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = null,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    public static new ApiResponse ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = null,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

