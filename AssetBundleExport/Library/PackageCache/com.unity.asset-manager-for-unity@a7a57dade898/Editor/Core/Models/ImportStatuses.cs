using System.Collections.Generic;

namespace Unity.AssetManager.Core.Editor
{
    class ImportStatuses : Dictionary<AssetIdentifier, ImportAttribute.ImportStatus>
    {
        public ImportStatuses(IEnumerable<ImportStatuses> importStatuses)
        {
            if (importStatuses == null)
                return;
            
            foreach (var importStatus in importStatuses)
            {
                AddRange(importStatus);
            }
        }

        public ImportStatuses() { }

        public void AddRange(ImportStatuses other)
        {
            foreach (var kvp in other)
            {
                if (ContainsKey(kvp.Key))
                {
                    Utilities.DevLogWarning("ImportStatusResults.AddRange: Duplicate key found. Overwriting existing value.");
                }

                this[kvp.Key] = kvp.Value;
            }
        }
    }
}
