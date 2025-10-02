using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A Service Error was returned by a service.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.Unknown)]
    [ServiceError(ServiceErrorCode.RequestRejected, HttpStatusCode.InternalServerError)]
    [ServiceError(ServiceErrorCode.UnprocessableEntity)]
    [ServiceError(ServiceErrorCode.Conflict)]
    [ServiceError(ServiceErrorCode.FailedDependency, HttpStatusCode.FailedDependency)]
    [ServiceError(ServiceErrorCode.MethodNotAllowed, HttpStatusCode.BadRequest)]
class ServiceException : Exception
    {
        internal class ErrorMessageDetail
        {
            public string errorCode { get; set; }
            public string errorMessage { get; set; }
        }

        ServiceError m_ServiceError { get; set; } = new () { Code = ServiceErrorCode.Unknown };
        IEnumerable<string> m_Details;

        /// <summary>
        /// The service error title.
        /// </summary>
        public string Title => m_ServiceError.Title;

        /// <summary>
        /// The ID of the failed request.
        /// </summary>
        public string RequestId => m_ServiceError.RequestId;

        /// <summary>
        /// The service error detail.
        /// </summary>
        public string Detail => m_ServiceError.Detail;

        /// <summary>
        /// An array of additional error m_Details.
        /// </summary>
        public IEnumerable<string> Details => m_Details;

        /// <summary>
        /// The service error code.
        /// </summary>
        public ServiceErrorCode ErrorCode => m_ServiceError.Code;

        /// <summary>
        /// The service error HTTP status code.
        /// </summary>
        public HttpStatusCode? StatusCode => m_ServiceError.Status;

        /// <summary>
        /// The service error type.
        /// </summary>
        public string Type => m_ServiceError.Type;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ServiceException() {}

        /// <summary>
        /// Creates and returns a <see cref="ServiceException"/> from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal ServiceException(ServiceError error) : base(error.ToString())
        {
            SetServiceError(error);
        }

        /// <summary>
        /// Creates and returns a <see cref="ServiceException"/> from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal ServiceException(ServiceError error, Exception innerException) : base(error.ToString(), innerException)
        {
            SetServiceError(error);
        }

        /// <summary>
        /// Creates and returns a <see cref="ServiceException"/> from the provided error message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ServiceException(string message)
            : base(message) { }

        /// <summary>
        /// Creates and returns a <see cref="ServiceException"/> from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Creates and returns a <see cref="ServiceException"/> from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">the streaming context.</param>
        protected ServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var error = (ServiceError) info.GetValue(nameof(m_ServiceError), typeof(ServiceError));
            SetServiceError(error);
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(m_ServiceError), m_ServiceError, typeof(ServiceError));
        }

        void SetServiceError(ServiceError serviceError)
        {
            m_ServiceError = serviceError;
            if (serviceError.Details != null && serviceError.Details.Any())
                m_Details = serviceError.Details.Select(d => d.ToString()).ToList();
        }
    }

    /// <summary>
    /// The request could not be processed.
    /// </summary>
    [Serializable]
class ServiceClientException : ServiceException
    {
        internal const string k_DefaultMessage = "The request could not be processed";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceClientException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal ServiceClientException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal ServiceClientException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ServiceClientException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceClientException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ServiceClientException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// This exception is thrown if the connection to the server fails.
    /// </summary>
    [Serializable]
class ConnectionException : ServiceException
    {
        internal const string k_DefaultMessage = "A connection to the server could not be established.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal ConnectionException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal ConnectionException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ConnectionException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConnectionException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// Not authorized to access service.
    /// </summary>
    [Serializable]
    [ServiceError(HttpStatusCode.Unauthorized)]
class UnauthorizedException : ServiceException
    {
        internal const string k_DefaultMessage = "Not authorized to access service.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UnauthorizedException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal UnauthorizedException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal UnauthorizedException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UnauthorizedException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// Access forbidden.
    /// </summary>
    [Serializable]
    [ServiceError(HttpStatusCode.Forbidden)]
    [ServiceError(ServiceErrorCode.Forbidden, HttpStatusCode.Forbidden)]
    [ServiceError(ServiceErrorCode.NoPermission, HttpStatusCode.Forbidden)]
class ForbiddenException : ServiceException
    {
        internal const string k_DefaultMessage = "Access forbidden.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForbiddenException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal ForbiddenException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal ForbiddenException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ForbiddenException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// Authentication failed.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.InvalidToken, HttpStatusCode.Unauthorized)]
    [ServiceError(ServiceErrorCode.TokenExpired, HttpStatusCode.Unauthorized)]
class AuthenticationFailedException : ServiceException
    {
        internal const string k_DefaultMessage = "Authentication failed.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AuthenticationFailedException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal AuthenticationFailedException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal AuthenticationFailedException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public AuthenticationFailedException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AuthenticationFailedException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AuthenticationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// The service or resource was not found.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.ApiMissing, HttpStatusCode.NotFound)]
    [ServiceError(ServiceErrorCode.NotFound, HttpStatusCode.NotFound)]
    [ServiceError(HttpStatusCode.NotFound)]
class NotFoundException : ServiceException
    {
        internal const string k_DefaultMessage = "The service or resource was not found.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotFoundException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal NotFoundException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal NotFoundException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NotFoundException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// Invalid argument.
    /// </summary>
    [Serializable]
    [ServiceError(HttpStatusCode.BadRequest)]
class InvalidArgumentException : ServiceException
    {
        internal const string k_DefaultMessage = "Invalid argument.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InvalidArgumentException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal InvalidArgumentException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal InvalidArgumentException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidArgumentException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidArgumentException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// The server encountered an unexpected error.
    /// </summary>
    [Serializable]
    [ServiceError(HttpStatusCode.InternalServerError)]
class ServerException : ServiceException
    {
        internal const string k_DefaultMessage = "The server encountered an unexpected error.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal ServerException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal ServerException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ServerException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServerException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ServerException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// Reaching the service has failed.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.TransportError, HttpStatusCode.InternalServerError)]
    [ServiceError(ServiceErrorCode.Timeout, HttpStatusCode.GatewayTimeout)]
    [ServiceError(ServiceErrorCode.ServiceUnavailable, HttpStatusCode.ServiceUnavailable)]
class TransientServiceException : ServiceException
    {
        internal const string k_DefaultMessage = "Reaching the service has failed.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TransientServiceException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal TransientServiceException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal TransientServiceException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransientServiceException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TransientServiceException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected TransientServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// Too many requests.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.TooManyRequests, HttpStatusCode.TooManyRequests)]
class TooManyRequestsException : ServiceException
    {
        internal const string k_DefaultMessage = "Too many requests.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TooManyRequestsException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal TooManyRequestsException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal TooManyRequestsException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TooManyRequestsException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TooManyRequestsException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected TooManyRequestsException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// The request was not valid.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.InvalidRequest, HttpStatusCode.BadRequest)]
class InvalidRequestException : ServiceException
    {
        internal const string k_DefaultMessage = "The request was not valid.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InvalidRequestException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal InvalidRequestException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal InvalidRequestException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidRequestException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidRequestException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// The payload is too large.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.PayloadTooLarge, HttpStatusCode.BadRequest)]
class PayloadTooLargeException : ServiceException
    {
        internal const string k_DefaultMessage = "The payload is too large.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PayloadTooLargeException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal PayloadTooLargeException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal PayloadTooLargeException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public PayloadTooLargeException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PayloadTooLargeException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected PayloadTooLargeException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }

    /// <summary>
    /// The media type is not supported.
    /// </summary>
    [Serializable]
    [ServiceError(ServiceErrorCode.UnsupportedMediaType, HttpStatusCode.UnsupportedMediaType)]
class UnsupportedMediaTypeException : ServiceException
    {
        internal const string k_DefaultMessage = "The media type is not supported.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UnsupportedMediaTypeException()
            : base(k_DefaultMessage) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/>.
        /// </summary>
        /// <param name="error">The service error.</param>
        internal UnsupportedMediaTypeException(ServiceError error)
            : base(error) {}

        /// <summary>
        /// Creates an instance from the provided <see cref="ServiceError"/> and inner exception.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal UnsupportedMediaTypeException(ServiceError error, Exception innerException)
            : base(error, innerException) {}

        /// <summary>
        /// Creates an instance from the provided error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UnsupportedMediaTypeException(string message)
            : base(message) {}

        /// <summary>
        /// Creates an instance from the provided error message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnsupportedMediaTypeException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Creates an instance from the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnsupportedMediaTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}
