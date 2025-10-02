namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Json formatted response from service.
    /// </summary>
    class ChannelRequest
    {
        /// <summary>
        /// The registered display name of the application making the channel request.
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// The string identifier that triggers a response in the channel.
        /// </summary>
        public string Request { get; set; }
        /// <summary>
        /// The <see cref="RequestData"/> for the channel.
        /// </summary>
        public RequestData RequestData { get; set; }
    }

    /// <summary>
    /// Json formatted additional data of the request
    /// </summary>
    class RequestData
    {
        /// <summary>
        /// The additional data needed to complete the request
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// Json formatted response from service.
    /// </summary>
    internal class ChannelIdResponseJson
    {
        /// <summary>
        /// The unique identifier of the created channel.
        /// </summary>
        public string channelId { get; set; }
    }

    /// <summary>
    /// Json formatted response from service.
    /// </summary>
    internal class ChannelJson
    {
        /// <summary>
        /// The <see cref="ChannelInfo"/> for the created channel.
        /// </summary>
        public ChannelInfo channel{ get; set; }
    }

    /// <summary>
    /// Json formatted response from service.
    /// </summary>
    class ChannelInfo
    {
        /// <summary>
        /// The unique identifier of the created channel.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The <see cref="ChannelRequest"/> of the created channel.
        /// </summary>
        public ChannelRequest Request { get; set; }
        /// <summary>
        /// The string response sent back to this channel.
        /// </summary>
        public string Response { get; set; }
    }

    /// <summary>
    /// A struct to identity channel service requests.
    /// </summary>
    struct ChannelServiceRequest
    {
        /// <summary>
        /// The relative route identifying the targeted resource.
        /// </summary>
        public string Route { get; internal set; }
        /// <summary>
        /// The action verb identifying the request on the targeted resource
        /// </summary>
        public string Action { get; internal set; }
    }

    /// <summary>
    /// Static definitions of supported <see cref="ChannelServiceRequest"/>.
    /// </summary>
    internal static class ChannelServiceRequestDefinition
    {
        /// <summary>
        /// A <see cref="ChannelServiceRequest"/> to initiate a PKCE login flow.
        /// </summary>
        public static ChannelServiceRequest PKCE_LOGIN => new()
        {
            Route = "",
            Action = "PKCE_LOGIN"
        };
    }
}
