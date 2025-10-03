using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// Output logs into Unity's logging mechanism (Debug.unityLogger.Log)
    /// </summary>
    class UnityLogOutput: ILogOutput
    {
        const string k_LogLevelEnvironmentVariableName = "UC_LOG_LEVEL";

        /// <summary>
        /// Set's up the <see cref="UnityLogOutput"/> at runtime.
        /// </summary>
        /// <remarks>Called automatically via <see cref="RuntimeInitializeOnLoadMethodAttribute"/> attribute with <see cref="RuntimeInitializeLoadType.SubsystemRegistration"/></remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void SetupUnityLogOutputRuntime()
        {
            SetupUnityLogOutput();
        }

        /// <summary>
        /// Clears any previous <see cref="ILogOutput"/> from <see cref="LogOutputs"/> and adds the <see cref="UnityLogOutput"/>.
        /// </summary>
        /// <remarks>Called automatically via <see cref="InitializeOnLoadMethod"/> attribute.</remarks>
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        public static void SetupUnityLogOutput()
        {
            LogOutputs.RemoveAll<UnityLogOutput>();
            LogOutputs.Add(new UnityLogOutput());
        }

        /// <summary>
        /// Whether logging is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The current <see cref="LogLevel"/>.
        /// </summary>
        public LogLevel CurrentLevel  { get; set; } = GetDefaultLogLevel();

        /// <summary>
        /// Writes a <see cref="LogEvent"/>.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        public void Write(LogEvent logEvent)
        {
            if(!Enabled)
                return;

            string message;
            if (logEvent.MessageArgs == null || logEvent.MessageArgs.Length == 0)
            {
                message = logEvent.Message;
            }
            else
            {
                message = string.Format(logEvent.Message, logEvent.MessageArgs);
            }

            if (logEvent.Exception != null)
            {
                message += "\n" + GenerateMessageFromException(logEvent.Exception);
            }

            UnityEngine.Object context = null;
            if (logEvent.Properties != null && logEvent.Properties.TryGetValue(UnityLoggerProvider.UnityObjectContextPropertyName, out var value))
            {
                context = value as UnityEngine.Object;
            }

            Debug.unityLogger.Log(ToUnityLogType(logEvent.Level), logEvent.LoggerName, message, context);
        }

        LogType ToUnityLogType(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Warning:
                    return LogType.Warning;
                case LogLevel.Error:
                    return LogType.Error;
                case LogLevel.Critical:
                    return LogType.Error;
            }

            return LogType.Log;
        }

        string GenerateMessageFromException(Exception exception)
        {
            var messageStringBuilder = new StringBuilder(exception.Message);
            var lines = exception.StackTrace?.Split('\n');

            if (lines != null)
            {
                foreach (var line in lines)
                {
                    var callInfo = line.Split("in ");
                    messageStringBuilder.Append("\n" + callInfo[0]);

                    if (callInfo.Length > 1)
                    {
                        var codeInfo = callInfo[1].Split(".cs:");
                        if (codeInfo.Length > 1)
                        {
                            messageStringBuilder.Append("in " + "<a href=\"" + codeInfo[0] + ".cs\" line=\"" + codeInfo[1] + "\">" +
                                                        codeInfo[0] + ".cs:" + codeInfo[1] + "</a>");
                        }
                        else
                        {
                            // exceptions inside a System.Task don't have ".cs:" in the message, so append as-is
                            messageStringBuilder.Append(codeInfo[0]);
                        }
                    }
                }
            }

            return messageStringBuilder.ToString();
        }

        static LogLevel GetDefaultLogLevel()
        {
            var logLevel = LogLevel.Information;

            // Check for compile time defines
            // The most verbose log level will be used if multiple are defined
            // (i.e. Trace > Debug > Information > Warning > Error > Critical)

#if UC_LOG_LEVEL_TRACE
            logLevel = LogLevel.Trace;
#elif UC_LOG_LEVEL_DEBUG
            logLevel = LogLevel.Debug;
#elif UC_LOG_LEVEL_INFORMATION
            logLevel = LogLevel.Information;
#elif UC_LOG_LEVEL_WARNING
            logLevel = LogLevel.Warning;
#elif UC_LOG_LEVEL_ERROR
            logLevel = LogLevel.Error;
#elif UC_LOG_LEVEL_CRITICAL
            logLevel = LogLevel.Critical;
#endif
            // Environment variable should always override defines if it exists
            var envLogLevel = Environment.GetEnvironmentVariable(k_LogLevelEnvironmentVariableName);
            if (Enum.TryParse<LogLevel>(envLogLevel, true, out var envLevel))
                logLevel = envLevel;

            return logLevel;
        }
    }
}
