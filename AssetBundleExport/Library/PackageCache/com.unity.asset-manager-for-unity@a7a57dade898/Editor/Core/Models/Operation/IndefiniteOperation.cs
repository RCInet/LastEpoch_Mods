using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class IndefiniteOperation : AssetDataOperation
    {
        public override float Progress => 0.0f;
        public override string OperationName => "Processing";
        public override string Description => "Processing";
        public override AssetIdentifier Identifier => m_AssetData.Identifier;
        public override bool StartIndefinite => true;
        public override bool ShowInBackgroundTasks => false;

        [SerializeReference]
        BaseAssetData m_AssetData;

        public IndefiniteOperation(BaseAssetData assetData)
        {
            m_AssetData = assetData;
        }
    }
}
