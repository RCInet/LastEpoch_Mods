using System;
using System.IO;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Creates an instance of IHttpRequestUriModifier from a file path.
    /// </summary>
    static class HttpRequestUriModifierFactory
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger(typeof(HttpRequestUriModifierFactory).FullName);

        const string k_EnvironmentVariable = "UNITY_CLOUD_SERVICES_REQUEST_MODIFIER";

        /// <summary>
        /// Creates the <see cref="IHttpRequestUriModifier"/> from a schema found at the file path provided via the environment variable <c>UNITY_CLOUD_MIDDLEWARE_MODIFIER</c>.
        /// </summary>
        /// <returns>The <see cref="IHttpRequestUriModifier"/> built with the schema if one was provided.</returns>
        public static IHttpRequestUriModifier CreateFromEnvironmentVariable()
        {
            var environmentVariableValue = Environment.GetEnvironmentVariable(k_EnvironmentVariable);
            if (string.IsNullOrEmpty(environmentVariableValue))
                return null;

            s_Logger.LogDebug($"Found environment variable {k_EnvironmentVariable} with content {environmentVariableValue}.");
            return CreateFromPath(environmentVariableValue);
        }

        /// <summary>
        /// Creates the <see cref="IHttpRequestUriModifier"/> from a schema found at the file path provided.
        /// </summary>
        /// <returns>The <see cref="IHttpRequestUriModifier"/> built with the modification schema if one was provided.</returns>
        public static IHttpRequestUriModifier CreateFromPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                s_Logger.LogWarning($"Unable to create a {nameof(HttpRequestUriModifier)} from empty file path.");
                return null;
            }

            if (!File.Exists(filePath))
            {
                s_Logger.LogWarning($"Unable to create a {nameof(HttpRequestUriModifier)} from file {filePath}. File does not exists.");
                return null;
            }

            HttpRequestUriModifier result = null;
            try
            {
                var fileContent = File.ReadAllText(filePath);
                result = new HttpRequestUriModifier(fileContent);
            }
            catch (Exception)
            {
                s_Logger.LogWarning($"Unable to create a {nameof(HttpRequestUriModifier)} from file {filePath}. Check file content.");
            }

            return result;
        }
    }
}
