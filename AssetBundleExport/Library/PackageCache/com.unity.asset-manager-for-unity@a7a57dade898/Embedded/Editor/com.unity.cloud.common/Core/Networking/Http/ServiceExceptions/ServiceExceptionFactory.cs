using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A factory for creating <see cref="ServiceException"/>s.
    /// </summary>
    static class ServiceExceptionFactory
    {
        // Cache exception mappings to avoid reflection on every call.
        static readonly Dictionary<(ServiceErrorCode? ErrorCode, HttpStatusCode? HttpStatusCode), Type> k_ServiceErrorToException = new ();

        static ServiceExceptionFactory()
        {
            CacheExceptionMaps();
        }

        /// <summary>
        /// Creates an exceptionType corresponding to the error statusCode contained in the provided <see cref="ServiceError"/>.
        /// If the statusCode is not known or 0, a more generic <see cref="ServiceException"/> will be returned.
        /// </summary>
        /// <param name="error">The provided service error.</param>
        /// <returns>The created exceptionType.</returns>
        /// <exceptionType cref="ArgumentNullException">Thrown when error is null.</exceptionType>
        public static ServiceException Build(ServiceError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            if (!k_ServiceErrorToException.Any())
                CacheExceptionMaps();

            // Set flags for reflection
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            // First try to find an exceptionType for the error code and HTTP status code tuple.
            if (TryGetExceptionTypeByServiceError(error, out var exceptionType))
                return (ServiceException)Activator.CreateInstance(exceptionType, flags, null, new object[] {error}, null);

            // If that fails, try to find an exceptionType for the error code alone.
            if (error.Code != ServiceErrorCode.Unknown && TryGetExceptionTypeByErrorCode(error, out exceptionType))
                return (ServiceException)Activator.CreateInstance(exceptionType, flags, null, new object[] {error}, null);

            // If that fails, try to find an exceptionType for the HTTP status code alone.
            if (TryGetExceptionTypeByHttpStatusCode(error, out exceptionType))
                return (ServiceException)Activator.CreateInstance(exceptionType, flags, null, new object[] {error}, null);

            // Default to a generic ServiceException.
            return new ServiceException(error);
        }

        /// <summary>
        /// Try to map a tuple of <see cref="ServiceErrorCode"/> and <see cref="HttpStatusCode"/> to an exception type.
        /// </summary>
        /// <param name="error">The <see cref="ServiceError"/> to map.</param>
        /// <param name="exceptionType">The output exception type. Null if none found.</param>
        /// <returns>Whether a mapping could be found.</returns>
        static bool TryGetExceptionTypeByServiceError(ServiceError error, out Type exceptionType)
        {
            var serviceErrorCodes = (error.Code, error.Status);
            return k_ServiceErrorToException.TryGetValue(serviceErrorCodes, out exceptionType);
        }

        /// <summary>
        /// Try to map a singular <see cref="ServiceErrorCode"/> to an exception type.
        /// </summary>
        /// <param name="error">The <see cref="ServiceError"/> to map.</param>
        /// <param name="exceptionType">The output exception type. Null if none found.</param>
        /// <returns>Whether a mapping could be found.</returns>
        static bool TryGetExceptionTypeByErrorCode(ServiceError error, out Type exceptionType)
        {
            exceptionType = k_ServiceErrorToException.FirstOrDefault(serviceErrorTuple => serviceErrorTuple.Key.ErrorCode == error.Code).Value;
            return exceptionType != null;

        }

        /// <summary>
        /// Try to map a singular <see cref="HttpStatusCode"/> to an exception type.
        /// </summary>
        /// <param name="error">The <see cref="ServiceError"/> to map.</param>
        /// <param name="exceptionType">The output exception type. Null if none found.</param>
        /// <returns>Whether a mapping could be found.</returns>
        static bool TryGetExceptionTypeByHttpStatusCode(ServiceError error, out Type exceptionType)
        {
            exceptionType = k_ServiceErrorToException.FirstOrDefault(serviceErrorTuple => serviceErrorTuple.Key.HttpStatusCode == error.Status &&
                serviceErrorTuple.Key.ErrorCode is null or ServiceErrorCode.Unknown).Value;
            if (exceptionType != null)
                return true;

            // If no exception type was found for the status code, use the HTTP status range.
            var statusInt = (int)error.Status;
            switch (statusInt)
            {
                case >= 400 and < 500:
                    k_ServiceErrorToException[(null, error.Status)] = typeof(ServiceClientException);
                    exceptionType = typeof(ServiceClientException);
                    return true;
                case >= 500:
                    k_ServiceErrorToException[(null, error.Status)] = typeof(ServerException);
                    exceptionType = typeof(ServerException);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Cache the code-to-exception maps to avoid reflection on every call.
        /// </summary>
        static void CacheExceptionMaps()
        {
            k_ServiceErrorToException.Clear();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(ServiceErrorAttribute), false)
                    .Cast<ServiceErrorAttribute>();

                foreach (var attribute in attributes)
                        k_ServiceErrorToException[(attribute.ErrorCode, attribute.HttpStatusCode)] = type;
            }
        }
    }
}
