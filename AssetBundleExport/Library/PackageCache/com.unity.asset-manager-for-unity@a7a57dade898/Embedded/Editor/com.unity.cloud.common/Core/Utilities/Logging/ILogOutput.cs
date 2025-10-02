namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Interface to implement to be able to receive log and act upon them (e.g. write to file)
    /// </summary>
    interface ILogOutput
    {
        /// <summary>
        /// Whether logging is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The current <see cref="LogLevel"/>.
        /// </summary>
        LogLevel CurrentLevel { get; set; }

        /// <summary>
        /// Write a <see cref="LogEvent"/>.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        void Write(LogEvent logEvent);
    }
}
