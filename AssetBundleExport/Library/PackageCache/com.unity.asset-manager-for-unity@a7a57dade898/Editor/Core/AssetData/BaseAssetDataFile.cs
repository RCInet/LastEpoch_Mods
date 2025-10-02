using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class BaseAssetDataFile
    {
        [SerializeField]
        string m_Path;

        [SerializeField]
        string m_Extension;

        [SerializeField]
        bool m_Available;

        [SerializeField]
        string m_Description;

        [SerializeField]
        List<string> m_Tags = new();

        [SerializeField]
        long m_FileSize;

        [SerializeField]
        string m_Guid;

        public string Path
        {
            get => m_Path;
            protected set => m_Path = value;
        }

        public string Extension
        {
            get => m_Extension;
            protected set => m_Extension = value;
        }

        public bool Available
        {
            get => m_Available;
            protected set => m_Available = value;
        }

        public string Description
        {
            get => m_Description;
            protected set => m_Description = value;
        }

        public IReadOnlyCollection<string> Tags
        {
            get => m_Tags;
            protected set => m_Tags = value?.ToList();
        }

        public long FileSize
        {
            get => m_FileSize;
            protected set => m_FileSize = value;
        }

        public string Guid
        {
            get => m_Guid;
            protected set => m_Guid = value;
        }
    }
}
