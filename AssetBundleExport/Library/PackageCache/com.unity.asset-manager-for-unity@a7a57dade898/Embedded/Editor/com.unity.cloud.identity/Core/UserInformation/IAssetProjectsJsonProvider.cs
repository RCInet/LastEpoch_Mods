using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IAssetProjectsJsonProvider
    {
        IAsyncEnumerable<AssetProjectJson> GetAssetProjectsJsonAsync(CancellationToken cancellationToken);
    }
}
