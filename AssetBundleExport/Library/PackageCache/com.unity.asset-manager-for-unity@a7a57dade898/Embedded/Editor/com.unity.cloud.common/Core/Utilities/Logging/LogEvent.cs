using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Object that holds the information associated to a single log
    /// </summary>
    class LogEvent
    {
        /// <summary>
        /// Initializes and returns a <see cref="LogEvent"/>.
        /// </summary>
        internal LogEvent() => Timestamp = DateTime.Now;

        /// <summary>
        /// Initializes and returns a <see cref="LogEvent"/>.
        /// </summary>
        /// <param name="loggerName">The logger's name.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The log message arguments.</param>
        /// <param name="properties">The properties to log.</param>
        internal LogEvent(string loggerName, LogLevel level, string message, object[] messageArgs, Dictionary<string, object> properties = null)
            : this()
        {
            LoggerName = loggerName;
            Level = level;
            Message = message;
            MessageArgs = messageArgs;
            Properties = properties;
        }

        /// <summary>
        /// Initializes and returns a <see cref="LogEvent"/>.
        /// </summary>
        /// <param name="loggerName">The logger's name.</param>
        /// <param name="level">The log level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The log message arguments.</param>
        /// <param name="properties">The properties to log.</param>
        internal LogEvent(string loggerName, LogLevel level, Exception exception, string message, object[] messageArgs, Dictionary<string, object> properties = null)
            : this(loggerName, level, message, messageArgs, properties)
        {
            Exception = exception;
        }

        /// <summary>
        /// The timestamp for the log.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// The logger's name.
        /// </summary>
        public string LoggerName { get; }

        /// <summary>
        /// The <see cref="LogLevel"/>.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// The log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The log message args.
        /// </summary>
        public object[] MessageArgs { get; }

        /// <summary>
        /// The properties to log.
        /// </summary>
        public Dictionary<string, object> Properties { get; }

        /// <summary>
        /// The exception to log.
        /// </summary>
        public Exception Exception { get; }
    }
}
