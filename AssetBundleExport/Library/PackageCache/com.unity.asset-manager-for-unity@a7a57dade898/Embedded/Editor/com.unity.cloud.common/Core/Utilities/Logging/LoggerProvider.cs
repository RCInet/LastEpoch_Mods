using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Main UCLogger class that allow retrieving/creating a UCLogger object
    /// </summary>
    static class LoggerProvider
    {
        /// <summary>
        /// Initializes and returns a logger with a specified name.
        /// </summary>
        /// <param name="typeFullName">The logger name.</param>
        /// <returns>The logger.</returns>
        public static UCLogger GetLogger(string typeFullName)
        {
            return new UCLogger(typeFullName);
        }

        /// <summary>
        /// Initializes and returns a logger for a specified type.
        /// </summary>
        /// <typeparam name="T">The type for the logger.</typeparam>
        /// <returns>The logger.</returns>
        public static UCLogger GetLogger<T>()
        {
            return new UCLogger(typeof(T).FullName);
        }

        /// <summary>
        /// Initializes and returns a logger for a specified type and property collection.
        /// </summary>>
        /// <param name="properties">The properties to be tracked by the logger.</param>
        /// <typeparam name="T">The type for the logger.</typeparam>
        /// <returns>The logger.</returns>
        public static UCLogger GetLogger<T>(IEnumerable<(string, object)> properties)
        {
            var logger = GetLogger<T>();
            foreach (var (name, value) in properties)
            {
                logger.AddOrUpdateProperty(name, value);
            }
            return logger;
        }
    }
}
