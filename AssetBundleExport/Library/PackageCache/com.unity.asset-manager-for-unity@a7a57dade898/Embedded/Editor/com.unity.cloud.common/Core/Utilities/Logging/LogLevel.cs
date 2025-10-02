namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Different levels of logging. Higher level have higher numerical value and thus are more important.
    /// </summary>
    enum LogLevel
    {
        /// <summary>
        /// Trace log level.
        /// Logs that contain the most detailed messages.
        /// </summary>
        Trace,

        /// <summary>
        /// Debug log level.
        /// Logs that are used for interactive investigation during development.
        /// </summary>
        /// <remarks>
        /// These logs should primarily contain information useful for debugging and have no long-term value.
        /// </remarks>
        Debug,

        /// <summary>
        /// Information log level.
        /// Logs that track the general flow of the application.
        /// </summary>
        /// <remarks>
        /// These logs should have long-term value.
        /// </remarks>
        Information,

        /// <summary>
        /// Warning log level.
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning,

        /// <summary>
        /// Error log level.
        /// Logs that highlight when the current flow of execution is stopped due to a failure.
        /// </summary>
        /// <remarks>
        /// These should indicate a failure in the current activity, not an application-wide failure.
        /// </remarks>
        Error,

        /// <summary>
        /// Critical log level.
        /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention.
        /// </summary>
        Critical,

        /// <summary>
        /// None log level.
        /// Not used for writing log messages. Specifies that a <see cref="ILogOutput"/> should not write any messages.
        /// </summary>
        None
    };
}
