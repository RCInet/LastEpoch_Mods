using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IMemberInfoJsonProvider
    {
        IAsyncEnumerable<MemberInfoJson> GetMemberInfoJsonAsync(Range range, CancellationToken cancellationToken);
    }
}
