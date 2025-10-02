using System;

namespace Unity.Cloud.AssetsEmbedded
{
    static class SearchCriteriaExtensions
    {
        internal static string BuildSearchKey(this string searchKey, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return string.IsNullOrEmpty(searchKey) ? "" : $"{searchKey}";
            }

            return string.IsNullOrEmpty(searchKey) ? $"{prefix}" : $"{prefix}.{searchKey}";
        }
    }
}
