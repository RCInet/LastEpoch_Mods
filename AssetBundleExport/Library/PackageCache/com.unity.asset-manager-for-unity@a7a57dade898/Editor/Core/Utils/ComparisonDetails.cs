using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Flags]
    enum ComparisonResults
    {
        None = 0,
        DataModified = 1 << 0,
        FilesAdded = 1 << 1,
        FilesRemoved = 1 << 2,
        FilesModified = 1 << 3,
        DependenciesAdded = 1 << 4,
        DependenciesRemoved = 1 << 5,
        DependenciesModified = 1 << 6,
        MetadataAdded = 1 << 7,
        MetadataRemoved = 1 << 8,
        MetadataModified = 1 << 9
    }

    [Serializable]
    struct ComparisonDetails
    {
        [SerializeField]
        ComparisonResults m_Results;

        [SerializeField]
        List<string> m_Messages;
        
        public ComparisonResults Results => m_Results;
        public IEnumerable<string> Messages => m_Messages ?? Enumerable.Empty<string>();

        public ComparisonDetails(ComparisonResults results, string message)
        {
            m_Results = results;
            m_Messages = !string.IsNullOrEmpty(message) ? new List<string> {message} : null;
        }
        
        ComparisonDetails(ComparisonResults results, IEnumerable<string> messages)
        {
            m_Results = results;
            m_Messages = messages?.ToList();
        }
        
        public static ComparisonDetails Merge(params ComparisonDetails[] results)
        {
            var details = ComparisonResults.None;
            var messages = new List<string>();
            foreach (var result in results)
            {
                details |= result.Results;
                messages.AddRange(result.Messages);
            }
            return new ComparisonDetails(details, messages.Distinct());
        }
    }
}
