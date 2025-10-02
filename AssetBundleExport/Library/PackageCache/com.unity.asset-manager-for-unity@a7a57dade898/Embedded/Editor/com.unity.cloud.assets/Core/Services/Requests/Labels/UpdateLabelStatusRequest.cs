using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class UpdateLabelStatusRequest : LabelRequest
    {
        public UpdateLabelStatusRequest(OrganizationId organizationId, string labelName, bool archive)
            : base(organizationId, labelName)
        {
            m_RequestUrl += archive ? "/archive" : "/unarchive";
        }
    }
}
