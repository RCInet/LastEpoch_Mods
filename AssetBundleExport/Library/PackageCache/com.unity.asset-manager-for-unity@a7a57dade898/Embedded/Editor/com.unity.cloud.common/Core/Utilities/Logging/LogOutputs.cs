using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Manages all log outputs
    /// </summary>
    static class LogOutputs
    {
        static List<ILogOutput> s_Outputs = new List<ILogOutput>();

        /// <summary>
        /// The current list of <see cref="ILogOutput"/>.
        /// </summary>
        public static IReadOnlyList<ILogOutput> Outputs => s_Outputs;

        /// <summary>
        /// Adds a <see cref="ILogOutput"/> to the list of outputs.
        /// </summary>
        /// <param name="logOutput">The log output to add.</param>
        public static void Add(ILogOutput logOutput) => s_Outputs.Add(logOutput);

        /// <summary>
        /// Returns whether the list of outputs contains a specific <see cref="ILogOutput"/>.
        /// </summary>
        /// <param name="logOutput">The log output to verify.</param>
        /// <returns>Whether the output is contained in the list.</returns>
        public static bool Contains(ILogOutput logOutput) => s_Outputs.Contains(logOutput);

        /// <summary>
        /// Returns whether the list of outputs contains any implementation of a <see cref="ILogOutput"/> with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ILogOutput"/> to verify.</typeparam>
        /// <returns>Whether the output type is contained in the list.</returns>
        public static bool Contains<T>() where T : ILogOutput => s_Outputs.OfType<T>().Any();

        /// <summary>
        /// Removes a <see cref="ILogOutput"/> from the list of outputs.
        /// </summary>
        /// <param name="logOutput">The log output to remove.</param>
        /// <returns>Whether the output was removed from the list.</returns>
        public static bool Remove(ILogOutput logOutput) => s_Outputs.Remove(logOutput);

        /// <summary>
        /// Removes all implementations of a <see cref="ILogOutput"/> with the specified type from the list of outputs.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ILogOutput"/> to remove.</typeparam>
        /// <returns>Whether any output was removed from the list.</returns>
        public static bool RemoveAll<T>() where T : ILogOutput
        {
            var removed = false;
            for (var i = s_Outputs.Count - 1; i >= 0; i--)
            {
                if (s_Outputs[i] is T)
                {
                    s_Outputs.RemoveAt(i);
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>
        /// Clear all log outputs.
        /// </summary>
        public static void Clear() => s_Outputs.Clear();

        internal static void Log(LogEvent logEvent)
        {
            var logEventLevel = logEvent.Level;
            if (logEventLevel == LogLevel.None)
                return;

            foreach (var logOutput in s_Outputs.Where(o => o.Enabled))
            {
                var logOutputLevel = logOutput.CurrentLevel;
                if (logEventLevel >= logOutputLevel)
                    logOutput.Write(logEvent);
            }
        }
    }
}
