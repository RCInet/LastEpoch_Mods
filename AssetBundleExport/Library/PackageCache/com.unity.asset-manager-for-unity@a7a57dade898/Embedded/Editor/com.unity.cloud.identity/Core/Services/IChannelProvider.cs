using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface to create request and Read response using the Unity Cloud Channel service.
    /// </summary>
    internal interface IChannelProvider
    {
        /// <summary>
        /// Creates an <see cref="IChannelProvider"/>.
        /// </summary>
        /// <param name="channelServiceRequest">The <see cref="ChannelServiceRequest"/></param>
        /// <param name="requestDetails">An optional string value to append additional information to the request.</param>
        /// <returns>A task that once completed returns the channel id of the created channel.</returns>
        Task<string> CreateChannelAsync(ChannelServiceRequest channelServiceRequest, string requestDetails = null);

        /// <summary>
        /// Creates an <see cref="IChannelProvider"/>.
        /// </summary>
        /// <param name="channelServiceRequest">The <see cref="ChannelServiceRequest"/></param>
        /// <param name="requestDetails">An optional string value to append additional information to the request.</param>
        /// <returns>A task that once completed returns the channel id of the created channel.</returns>
        Task<string> CreateChannelAsync(ChannelServiceRequest channelServiceRequest, object requestDetails);

        /// <summary>
        /// Gets the <see cref="ChannelInfo"/> response for a provided channel Id.
        /// </summary>
        /// <param name="channelId">The channel Id of the created channel to fetch.</param>
        /// <returns>A task that once completed returns the <see cref="ChannelInfo"/> response.</returns>
        Task<ChannelInfo> GetChannelAsync(string channelId);
    }
}
