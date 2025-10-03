using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for service environment.
    /// </summary>
    static class ServiceEnvironmentUtils
    {
        internal static ServiceEnvironment? ParseEnvironmentValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (Enum.TryParse<ServiceEnvironment>(value, true, out var env))
                return env;

            return null;
        }
    }
}
