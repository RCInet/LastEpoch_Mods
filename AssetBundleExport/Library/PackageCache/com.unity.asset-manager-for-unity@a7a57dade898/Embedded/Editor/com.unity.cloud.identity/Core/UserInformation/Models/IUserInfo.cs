using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that exposes user information.
    /// </summary>
    interface IUserInfo
    {
        /// <summary>
        /// The UserId of the user.
        /// </summary>
        UserId UserId { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
       string Name { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        string Email { get; set; }
    }
}
