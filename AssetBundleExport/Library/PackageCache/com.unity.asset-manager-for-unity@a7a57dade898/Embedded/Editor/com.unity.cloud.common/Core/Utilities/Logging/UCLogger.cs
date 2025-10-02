using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Class used to do the actual logging from dev code.
    /// </summary>
    class UCLogger
    {
        Dictionary<string, object> m_Properties; // for structured logging... used for the Unity object context

        /// <summary>
        /// The UCLogger's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The UCLogger's tracked properties.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => m_Properties;

        internal UCLogger(string name) => Name = name;

        /// <summary>
        /// Add or update a tracked property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property.</param>
        public void AddOrUpdateProperty(string name, object value)
        {
            EnsurePropertiesAreInitialized();
            m_Properties[name] = value;
        }

        /// <summary>
        /// Remove a tracked property.
        /// </summary>
        /// <param name="name">The property name.</param>
        public void RemoveProperty(string name)
        {
            if (m_Properties != null)
            {
                m_Properties.Remove(name);
            }
        }

        /// <summary>
        /// Log a message for a specified log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        internal void Log(LogLevel level, string message, params object[] messageArgs)
        {
            LogOutputs.Log(new LogEvent(Name, level, message, messageArgs, m_Properties));
        }

        /// <summary>
        /// Log an <see cref="Exception"/> for a specified log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        internal void Log(LogLevel level, Exception exception, string message, params object[] messageArgs)
        {
            LogOutputs.Log(new LogEvent(Name, level, exception, message, messageArgs, m_Properties));
        }

        void EnsurePropertiesAreInitialized()
        {
            if (m_Properties == null)
            {
                m_Properties = new Dictionary<string, object>();
            }
        }
    }
}
