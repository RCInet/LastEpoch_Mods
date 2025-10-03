using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class UserMetadata : MetadataValue
    {
        public UserId UserId { get; set; }

        public UserMetadata(UserId value = default)
            : base(MetadataValueType.User)
        {
            UserId = value;
        }

        internal UserMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            var userId = value?.ToString() ?? string.Empty;
            UserId = string.IsNullOrEmpty(userId) ? UserId.None : new UserId(userId);
        }

        internal override object GetValue()
        {
            return UserId.ToString();
        }
    }
}
