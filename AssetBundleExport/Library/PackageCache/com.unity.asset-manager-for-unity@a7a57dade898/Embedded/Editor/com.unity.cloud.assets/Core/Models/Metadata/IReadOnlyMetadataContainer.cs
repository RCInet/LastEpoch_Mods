using System;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IReadOnlyMetadataContainer
    {
        /// <summary>
        /// Returns a <see cref="MetadataQueryBuilder"/> for filtering and fetching metadata.
        /// </summary>
        /// <returns>A <see cref="MetadataQueryBuilder"/> for defining and executing queries. </returns>
        MetadataQueryBuilder Query();
    }
}
