using System;
using System.Net;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An attribute for defining a <see cref="ServiceError"/> for a specific error code and HTTP status code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class ServiceErrorAttribute : Attribute
    {
        /// <summary>
        /// The error code of the service error.
        /// </summary>
        public ServiceErrorCode? ErrorCode { get; }

        /// <summary>
        /// The HTTP status code of the service error.
        /// </summary>
        public HttpStatusCode? HttpStatusCode { get; }

        /// <summary>
        /// Creates an instance of the <see cref="ServiceErrorAttribute"/> class.
        /// </summary>
        /// <param name="errorCode">The service error's code.</param>
        /// <param name="httpStatusCode">The service error's http status code.</param>
        public ServiceErrorAttribute(ServiceErrorCode errorCode, HttpStatusCode httpStatusCode)
        {
            ErrorCode = errorCode;
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Creates an instance of the <see cref="ServiceErrorAttribute"/> class.
        /// </summary>
        /// <param name="errorCode">The service error's code.</param>
        public ServiceErrorAttribute(ServiceErrorCode errorCode)
        {
            ErrorCode = errorCode;
            HttpStatusCode = null;
        }

        /// <summary>
        /// Creates an instance of the <see cref="ServiceErrorAttribute"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        public ServiceErrorAttribute(HttpStatusCode statusCode)
        {
            ErrorCode = null;
            HttpStatusCode = statusCode;
        }
    }
}
