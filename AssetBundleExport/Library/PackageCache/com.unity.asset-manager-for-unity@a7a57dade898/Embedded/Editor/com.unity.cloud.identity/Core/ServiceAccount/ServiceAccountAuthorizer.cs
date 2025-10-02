using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An <see cref="IServiceAuthorizer"/> implementation that expects service account credentials from a provided launch argument or environment variable.
    /// </summary>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/ServiceAccountAuthorizerExample.cs" region="ServiceAccountAuthorizer"/>
    /// </example>
    class ServiceAccountAuthorizer : IServiceAuthorizer
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ServiceAccountAuthorizer>();

        string m_AccountCredentials = string.Empty;

        /// <summary>
        /// Returns the expected key name in launch arguments that holds the service account credentials in the &lt;key id&gt;:&lt;secret key&gt; format.
        /// </summary>
        public static readonly string s_ServiceAccountKeyName = "UNITY_SERVICE_ACCOUNT_CREDENTIALS";

        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;

        /// <summary>
        /// Returns an <see cref="IAuthenticator"/> implementation that expects service account credentials from a provided launch argument or environment variable.
        /// </summary>
        /// <remarks>
        /// The Unity service account must be created for the correct organization and have the correct permissions to access Unity Cloud APIs.
        /// </remarks>
        /// <param name="authenticationPlatformSupport">The <see cref="IAuthenticationPlatformSupport"/> that handles credential injection.</param>
        /// <exception cref="InvalidOperationException">Thrown if no service account credentials are provided as a launch argument or environment variable.</exception>
        [Obsolete("Deprecated in favour of ServiceAccountAuthenticator.")]
        public ServiceAccountAuthorizer(IAuthenticationPlatformSupport authenticationPlatformSupport)
        {
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            ParseCredentials();
        }

        void ParseCredentials()
        {
            var keyName = $"-{s_ServiceAccountKeyName}";

            // If launch arguments key value pairs contains the token key name
            if (m_AuthenticationPlatformSupport != null && m_AuthenticationPlatformSupport.ActivationKeyValue.Count > 0 && m_AuthenticationPlatformSupport.ActivationKeyValue.TryGetValue(keyName, out var value))
            {
                s_Logger.LogDebug($"Service account credentials provided from CLI -{s_ServiceAccountKeyName} key value pair");
                m_AccountCredentials = ToBase64(value);
            }
            else
            {
                // Otherwise look at Environment variables for value
                var envServiceAccountValue = Environment.GetEnvironmentVariable(s_ServiceAccountKeyName);
                if (!string.IsNullOrEmpty(envServiceAccountValue))
                {
                    s_Logger.LogDebug($"Service account credentials provided from { s_ServiceAccountKeyName} environment variable");
                    m_AccountCredentials = ToBase64(envServiceAccountValue);
                    if (m_AuthenticationPlatformSupport != null && !string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl))
                    {
                        m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.ActivationUrl);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Cannot Initialize {nameof(IServiceAuthorizer)}. Missing -{s_ServiceAccountKeyName} value in launch arguments or {s_ServiceAccountKeyName} in environment variables.");
                }
            }
        }

        /// <inheritdoc cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer.AddAuthorization"/>
        public Task AddAuthorization(HttpHeaders headers)
        {
            headers.AddAuthorization(m_AccountCredentials, ServiceHeaderUtils.k_BasicScheme);
            return Task.CompletedTask;
        }

        static string ToBase64(string value)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }
    }
}
