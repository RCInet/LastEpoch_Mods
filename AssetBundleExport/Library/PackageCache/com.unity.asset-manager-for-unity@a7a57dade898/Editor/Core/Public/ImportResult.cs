using System.Collections.Generic;

namespace Unity.AssetManager.Editor
{
    /// <summary>
    /// The result of an import task.
    /// </summary>
    public struct ImportResult
    {
        /// <summary>
        /// The ids of the assets that were imported.
        /// </summary>
        public IEnumerable<string> ImportedAssetIds { get; internal set; }
    }
}
