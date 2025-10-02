using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helpers function for logging
    /// </summary>
    static class LoggerExtension
    {
        /// <summary>
        /// Log a message at the <see cref="LogLevel.Trace"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogTrace(this UCLogger logger, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Trace, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Trace"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        public static void LogTrace(this UCLogger logger, Exception exception)
            => logger.Log(LogLevel.Trace, exception, exception.Message);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Trace"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogTrace(this UCLogger logger, Exception exception, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Trace, exception, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogDebug(this UCLogger logger, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Debug, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        public static void LogDebug(this UCLogger logger, Exception exception)
            => logger.Log(LogLevel.Debug, exception, exception.Message);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogDebug(this UCLogger logger, Exception exception, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Debug, exception, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Information"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogInformation(this UCLogger logger, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Information, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Information"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        public static void LogInformation(this UCLogger logger, Exception exception)
            => logger.Log(LogLevel.Information, exception, exception.Message);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Information"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogInformation(this UCLogger logger, Exception exception, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Information, exception, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogWarning(this UCLogger logger, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Warning, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        public static void LogWarning(this UCLogger logger, Exception exception)
            => logger.Log(LogLevel.Warning, exception, exception.Message);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogWarning(this UCLogger logger, Exception exception, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Warning, exception, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogError(this UCLogger logger, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Error, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        public static void LogError(this UCLogger logger, Exception exception)
            => logger.Log(LogLevel.Error, exception, exception.Message);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogError(this UCLogger logger, Exception exception, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Error, exception, message, messageArgs);

        /// <summary>
        /// Log an <see cref="Exception"/> at the <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogCritical(this UCLogger logger, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Critical, message, messageArgs);

        /// <summary>
        /// Log a message at the <see cref="LogLevel.Critical"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        public static void LogCritical(this UCLogger logger, Exception exception)
            => logger.Log(LogLevel.Critical, exception, exception.Message);

        /// <summary>
        /// Log an <see cref="Exception"/> at the <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception"></param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void LogCritical(this UCLogger logger, Exception exception, string message, params object[] messageArgs)
            => logger.Log(LogLevel.Critical, exception, message, messageArgs);
    }
}
