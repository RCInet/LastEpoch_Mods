using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Enum of all known error codes that can be thrown by a service.
    /// </summary>
    enum ServiceErrorCode
    {
        // Don't forget to add any new error code to the documentation at docs/UnifiedErrors.md
        // New errors also need to be added to the HttpExceptionFactory, HttpErrorFactory and
        // HttpErrorExtensions.

        // X Legacy and unknown

        /// <summary>
        /// The server encountered an unspecified error.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// DNS, TLS, and other transport errors that return an invalid response.
        /// </summary>
        TransportError = 1,

        /// <summary>
        /// The request timed out because no response was received in the time provided.
        /// </summary>
        Timeout = 2,

        /// <summary>
        /// The service is unavailable or overloaded. Try again later.
        /// </summary>
        ServiceUnavailable = 3,

        /// <summary>
        /// The API does not exist.
        /// </summary>
        ApiMissing = 4,

        /// <summary>
        /// The request was rejected before reaching the API.
        /// </summary>
        RequestRejected = 5,

        /// <summary>
        /// You're making multiple requests too frequently. Make your requests at a lower rate.
        /// </summary>
        TooManyRequests = 50,

        /// <summary>
        /// The authentication token provided is invalid. Generate a new authentication token and try again.
        /// </summary>
        InvalidToken = 51,

        /// <summary>
        /// The authentication token provided is expired. Generate a new authentication token and try again.
        /// </summary>
        TokenExpired = 52,

        /// <summary>
        /// You don't have permission to perform this operation.
        /// </summary>
        Forbidden = 53,

        /// <summary>
        /// The target resource couldn't be found.
        /// </summary>
        NotFound = 54,

        /// <summary>
        /// Part of the request is invalid.
        /// </summary>
        InvalidRequest = 55,

        /// <summary>
        /// You don't have any permissions
        /// </summary>
        NoPermission = 56,

        /// <summary>
        /// The content type and syntax of your request is correct, but the instructions are unprocessable.
        /// </summary>
        UnprocessableEntity = 57,

        /// <summary>
        /// This request conflicts with the current state of the target resource.
        /// </summary>
        Conflict = 58,

        /// <summary>
        /// The requested action can't be performed because a dependent action has failed.
        /// </summary>
        FailedDependency = 59,

        /// <summary>
        /// The target resource doesn't support this request method.
        /// </summary>
        MethodNotAllowed = 60,

        /// <summary>
        /// The request entity is larger than the limits defined by the server.
        /// </summary>
        PayloadTooLarge = 61,

        /// <summary>
        /// The payload format is not supported.
        /// </summary>
        UnsupportedMediaType = 62,
    }
}
