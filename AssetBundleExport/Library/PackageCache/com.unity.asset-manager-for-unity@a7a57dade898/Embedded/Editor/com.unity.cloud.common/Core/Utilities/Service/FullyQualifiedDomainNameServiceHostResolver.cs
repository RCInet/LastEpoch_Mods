using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Resolves the service host and service requests URI based on provided fully qualified domain name and path prefix values.
    /// </summary>
    internal class FullyQualifiedDomainNameServiceHostResolver : IServiceHostResolver
    {
        readonly string m_FullyQualifiedDomainNameAndPathPrefix;

        /// <summary>
        /// Returns a ServiceHostResolver that always resolve the domain to the fully qualified name and path prefix provided in the constructor.
        /// </summary>
        /// <remarks>This implementation ignores all URL modifiers.</remarks>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name.</param>
        /// <param name="pathPrefix">The optional path prefix.</param>
        /// <exception cref="ArgumentException">Thrown when the provided path prefix does not start with '/'.</exception>
        public FullyQualifiedDomainNameServiceHostResolver(string fullyQualifiedDomainName, string pathPrefix = "/")
        {
            // Accepts the single "/" has a valid empty path prefix
            if (!string.IsNullOrEmpty(pathPrefix) && pathPrefix.Equals("/"))
                pathPrefix = string.Empty;

            // Throw if non-empty path prefix does not start with standard "/"
            if (!string.IsNullOrEmpty(pathPrefix) && !pathPrefix.StartsWith("/"))
                throw new ArgumentException("Path prefix must start with '/'");

            var withPathPrefix = string.IsNullOrEmpty(pathPrefix) ? string.Empty : pathPrefix;
            m_FullyQualifiedDomainNameAndPathPrefix = $"{fullyQualifiedDomainName}{withPathPrefix}";
        }


        /// <summary>
        /// Gets the resolved <see cref="ServiceEnvironment"/>.
        /// </summary>
        /// <remarks>The <see cref="FullyQualifiedDomainNameServiceHostResolver"/> does not support <see cref="ServiceEnvironment"/> value and will throw a <see cref="NotImplementedException"/> if this method is called.</remarks>
        /// <returns>The resolved environment.</returns>
        public ServiceEnvironment GetResolvedEnvironment()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the resolved the <see cref="ServiceDomainProvider"/>.
        /// </summary>
        /// /// <remarks>The <see cref="FullyQualifiedDomainNameServiceHostResolver"/> does not support <see cref="ServiceDomainProvider"/> value and will throw a <see cref="NotImplementedException"/> if this method is called.</remarks>
        /// <returns>The resolved service domain provider.</returns>
        public ServiceDomainProvider GetResolvedDomainProvider()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetResolvedAddress(ServiceProtocol protocol = ServiceProtocol.Http)
        {
            var uriScheme = protocol switch
            {
                ServiceProtocol.Http => "https",
                ServiceProtocol.WebSocket => "ws",
                ServiceProtocol.WebSocketSecure => "wss",
                _ => "https"
            };

            return $"{uriScheme}://{m_FullyQualifiedDomainNameAndPathPrefix}";
        }

        /// <inheritdoc/>
        public string GetResolvedRequestUri(string path, ServiceProtocol protocol = ServiceProtocol.Http)
        {
            return $"{GetResolvedAddress(protocol)}{path}";
        }
    }
}
