using System.Collections.Generic;
using System.Linq;

namespace Unity.AssetManager.Core.Editor
{
    static class SavedFilterUtilities
    {
        // Will return a name with the appropriate suffix index if the requested name already exists.
        internal static string GetValidFilterName(IEnumerable<SavedAssetSearchFilter> filters, string requestedName)
        {
            var filterName = requestedName;
            var index = 1;

            while (filters.Any(f => f.FilterName == filterName))
                filterName = $"{requestedName} {index++}";

            return filterName;
        }
    }
}
