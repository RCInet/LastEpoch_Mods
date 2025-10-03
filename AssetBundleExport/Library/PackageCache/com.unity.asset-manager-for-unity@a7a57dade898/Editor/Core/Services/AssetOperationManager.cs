using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IAssetOperationManager : IService
    {
        event Action<AssetDataOperation> OperationProgressChanged;
        event Action<AssetDataOperation> OperationFinished;
        event Action<TrackedAssetIdentifier> OperationCleared;
        event Action FinishedOperationsCleared;

        AssetDataOperation GetAssetOperation(AssetIdentifier identifier);
        void PauseAllOperations();
        void ResumeAllOperations();
        void RegisterOperation(AssetDataOperation operation);
        void ClearOperation(TrackedAssetIdentifier identifier);
        void ClearFinishedOperations();
    }

    [Serializable]
    class AssetOperationManager : BaseService<IAssetOperationManager>, IAssetOperationManager
    {
        readonly Dictionary<TrackedAssetIdentifier, AssetDataOperation> m_Operations = new();

        public event Action<AssetDataOperation> OperationProgressChanged;
        public event Action<AssetDataOperation> OperationFinished;
        public event Action<TrackedAssetIdentifier> OperationCleared;
        public event Action FinishedOperationsCleared;

        public AssetDataOperation GetAssetOperation(AssetIdentifier identifier)
        {
            return m_Operations.GetValueOrDefault(new TrackedAssetIdentifier(identifier));
        }

        public void PauseAllOperations()
        {
            foreach (var assetOperation in m_Operations.Values)
            {
                assetOperation.Pause();
            }
        }

        public void ResumeAllOperations()
        {
            foreach (var assetOperation in m_Operations.Values)
            {
                assetOperation.Resume();
            }
        }

        public void RegisterOperation(AssetDataOperation operation)
        {
            var identifier = new TrackedAssetIdentifier(operation.Identifier);

            if (m_Operations.TryGetValue(identifier, out var existingOperation))
            {
                // The operation is already registered
                if (operation == existingOperation)
                    return;

                if (!existingOperation.IsSticky)
                {
                    Debug.LogWarning("An operation for this asset already exists");
                }
                else
                {
                    ClearOperation(identifier);
                }
            }

            operation.ProgressChanged += _ => OperationProgressChanged?.Invoke(operation);
            operation.Finished += _ =>
            {
                if (!operation.IsSticky)
                {
                    ClearOperation(identifier);
                }

                OperationFinished?.Invoke(operation);
            };

            m_Operations[identifier] = operation;
        }

        public void ClearOperation(TrackedAssetIdentifier identifier)
        {
            if(m_Operations.TryGetValue(identifier, out var operation))
            {
                OperationCleared?.Invoke(identifier);
                operation.Remove();
                m_Operations.Remove(identifier);
            }
        }

        public void ClearFinishedOperations()
        {
            foreach (var operation in m_Operations.Values.ToArray())
            {
                if (operation.Status != OperationStatus.InProgress)
                {
                    ClearOperation(new TrackedAssetIdentifier(operation.Identifier));
                }
            }

            FinishedOperationsCleared?.Invoke();
        }
    }
}
