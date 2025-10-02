using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Handles resolving the correct override value for the underling enum type.
    /// System overrides are prioritized over application overrides.
    /// </summary>
    /// <typeparam name="T">The Service Enum type to resolve.</typeparam>
    class ServiceEnumOverride<T> where T : struct, Enum
    {
        readonly UCLogger s_Logger = LoggerProvider.GetLogger<ServiceEnumOverride<T>>();

        internal string OverrideValue;
        internal T? Result;

        internal void ResolveOverride(string systemOverride, string applicationOverride, Func<string, T?> parseValue)
        {
            var resolvedSystemOverride = parseValue(systemOverride);
            if (resolvedSystemOverride.HasValue)
            {
                OverrideValue = systemOverride;
                Result = resolvedSystemOverride;

                s_Logger.LogDebug($"{nameof(ServiceEnumOverride<T>)} created with system override value: {systemOverride} and {typeof(T).Name}: {Result}");
            }
            else
            {
                var resolveApplicationOverride = parseValue(applicationOverride);
                if (resolveApplicationOverride.HasValue)
                {
                    OverrideValue = applicationOverride;
                    Result = resolveApplicationOverride;

                    s_Logger.LogDebug($"{nameof(ServiceEnumOverride<T>)} created with application override value: {applicationOverride} and {typeof(T).Name}: {Result}");
                }
                else
                    s_Logger.LogDebug($"{nameof(ServiceEnumOverride<T>)} created without override value for {typeof(T).Name}");
            }
        }
    }
}
