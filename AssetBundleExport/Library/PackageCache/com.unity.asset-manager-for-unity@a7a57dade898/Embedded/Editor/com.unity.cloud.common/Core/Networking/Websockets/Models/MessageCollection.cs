using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Object used to store multiple Message objects and corresponding checkpoints
    /// </summary>
    [Serializable]
class MessageCollection
    {
        /// <summary>
        /// Initializes and returns an instance of <see cref="MessageCollection"/>
        /// </summary>
        /// <param name="messages">The messages to add to the collection.</param>
        /// <param name="checkpointEpochMilliseconds">The checkpoint to track the DateTime at which the MessageCollection is sent.</param>
        public MessageCollection(IEnumerable<Message> messages, long checkpointEpochMilliseconds)
        {
            Messages = messages.ToList();
            CheckpointEpochMilliseconds = checkpointEpochMilliseconds;
        }

        /// <summary>
        /// List of Message objects stored in the MessageCollection
        /// </summary>
        public List<Message> Messages { get; set; }

        /// <summary>
        /// Used to track the DateTime at which the MessageCollection is sent
        /// Generally tracked in UnixTimeMilliseconds
        /// </summary>
        public long CheckpointEpochMilliseconds { get; set; }
    }
}
