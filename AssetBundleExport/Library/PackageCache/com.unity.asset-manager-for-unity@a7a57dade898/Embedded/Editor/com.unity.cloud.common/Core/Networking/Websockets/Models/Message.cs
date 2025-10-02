using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Object used to contain messaging information
    /// </summary>
    [Serializable]
class Message
    {
        /// <summary>
        /// Version of the messaging type being used
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Type of message being sent
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Data that is embedded in the message
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Verifies that the Message object has valid entries for all its fields
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Version) &&
                !string.IsNullOrWhiteSpace(Type) &&
                !string.IsNullOrWhiteSpace(Payload);
        }
    }
}
