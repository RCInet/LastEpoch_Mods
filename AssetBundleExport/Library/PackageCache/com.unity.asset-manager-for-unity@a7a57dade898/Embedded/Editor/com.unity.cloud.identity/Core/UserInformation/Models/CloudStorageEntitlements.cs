using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class CloudStorageEntitlements : ICloudStorageEntitlements
    {
        public bool MeteredOptInEnabled { get; set; }

        internal CloudStorageEntitlements(CloudStorageEntitlementsJson cloudStorageEntitlementsJson)
        {
            MeteredOptInEnabled = cloudStorageEntitlementsJson.MeteredOptInEnabled;
        }
    }
}
