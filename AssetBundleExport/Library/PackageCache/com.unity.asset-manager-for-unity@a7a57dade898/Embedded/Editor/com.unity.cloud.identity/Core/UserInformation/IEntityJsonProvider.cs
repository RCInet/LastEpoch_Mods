using System.Collections.Generic;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IEntityJsonProvider
    {
        IEnumerable<EntityJson> GetEntityJsonAsync(string entityId, string entityType);
    }
}
