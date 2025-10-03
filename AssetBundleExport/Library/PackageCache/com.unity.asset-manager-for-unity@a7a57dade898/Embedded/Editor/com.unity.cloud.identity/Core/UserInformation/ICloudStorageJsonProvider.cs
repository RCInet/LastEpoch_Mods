using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface ICloudStorageJsonProvider
    {
        public Task<CloudStorageUsageJson> GetCloudStorageUsageAsync(CancellationToken cancellationToken);

        public Task<CloudStorageEntitlementsJson> GetCloudStorageEntitlementsAsync(CancellationToken cancellationToken);
    }
}
