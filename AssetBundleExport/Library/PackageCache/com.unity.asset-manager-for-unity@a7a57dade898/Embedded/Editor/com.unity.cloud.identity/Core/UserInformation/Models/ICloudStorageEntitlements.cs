using System.Collections.Generic;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface ICloudStorageEntitlements
    {
        public bool MeteredOptInEnabled { get; set; }
    }
}
