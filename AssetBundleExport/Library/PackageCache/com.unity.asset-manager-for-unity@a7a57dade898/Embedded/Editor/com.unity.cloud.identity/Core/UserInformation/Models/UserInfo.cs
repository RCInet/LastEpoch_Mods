
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class UserInfo : IUserInfo
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        public UserId UserId { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; }

        internal UserInfo()
        {
        }

        internal UserInfo(UnityUserInfoJson unityUserInfoJson)
        {
            UserId = new UserId(unityUserInfoJson.GenesisId);
            Name = unityUserInfoJson.Name;
            Email = unityUserInfoJson.Email;
        }
    }
}
