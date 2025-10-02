namespace Unity.Cloud.IdentityEmbedded
{
    internal class OpenIdConfigurationJson
    {
        public string authorization_endpoint { get; set; }
        public string token_endpoint { get; set; }
        public string userinfo_endpoint { get; set; }
        public string end_session_endpoint { get; set; }
    }
}
