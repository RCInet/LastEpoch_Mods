using System;
using System.Runtime.Serialization;

namespace Unity.AssetManager.Editor
{
    /// <summary>
    /// The exception that is thrown when an error occurs during the initialization of the Asset Manager.
    /// </summary>
    [Serializable]
    public class InitializationException : AssetManagerException
    {
        /// <summary><para>Initializes a new instance of the <see cref="T:Unity.AssetManager.InitializationException" /> class.</para></summary>
        public InitializationException()
            : base() { }

        /// <summary><para>Initializes a new instance of the <see cref="T:Unity.AssetManager.InitializationException" /> class.</para></summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InitializationException(string message)
            : base(message) { }

        /// <summary><para>Initializes a new instance of the <see cref="T:Unity.AssetManager.InitializationException" /> class.</para></summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InitializationException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Protected constructor used for serialization
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected InitializationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
