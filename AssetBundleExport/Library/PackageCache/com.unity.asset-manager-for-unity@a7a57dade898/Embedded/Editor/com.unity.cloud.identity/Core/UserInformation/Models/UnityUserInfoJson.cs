using System;
using System.Collections.Generic;

namespace Unity.Cloud.IdentityEmbedded
{
    [Serializable]
    internal class UnityUserInfoJson
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The genesis id of the user.
        /// </summary>
        public string GenesisId { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The list of <see cref="Organization"/> the user belongs to.
        /// </summary>
        public IEnumerable<OrganizationJson> Organizations { get; set; }
    }

    [Serializable]
    internal class UnityUserContextJson
    {
        /// <summary>
        /// The UnityUserInfoJson of the user.
        /// </summary>
        public UnityUserInfoJson User { get; set; }
    }
}
