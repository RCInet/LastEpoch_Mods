using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This interface contains all the information about a cloud project.
    /// </summary>
    interface IProjectBaseData
    {
        /// <summary>
        /// The project name.
        /// </summary>
        [DataMember(Name = "name")]
        string Name { get; }

        /// <summary>
        /// The project metadata.
        /// </summary>
        [DataMember(Name = "metadata")]
        Dictionary<string, string> Metadata { get; }
    }
}
