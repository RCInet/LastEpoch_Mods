namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// Unity-specific LogOutputs. Gives access to the UnityEngine.Object context used in Unity's log
    /// </summary>
    static class UnityLoggerProvider
    {
        /// <summary>
        /// The property name used for a <see cref="UnityEngine.Object"/> context.
        /// </summary>
        public static string UnityObjectContextPropertyName => "UnityObjectContext";

        /// <summary>
        /// Initializes and returns a <see cref="UCLogger"/> for a <see cref="UnityEngine.Object"/> context.
        /// </summary>
        /// <param name="context">The <see cref="UnityEngine.Object"/> context.</param>
        /// <typeparam name="T">The type for the logger.</typeparam>
        /// <returns></returns>
        public static UCLogger GetLogger<T>(UnityEngine.Object context)
        {
            return LoggerProvider.GetLogger<T>(new[] { (UnityObjectContextPropertyName, (object)context) });
        }
    }
}

