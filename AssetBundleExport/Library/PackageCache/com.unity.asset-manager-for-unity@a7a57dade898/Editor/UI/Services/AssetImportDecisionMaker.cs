using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
#if UNITY_STANDALONE_OSX
using UnityEditor;
#endif

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class AssetImportDecisionMaker : IAssetImportDecisionMaker
    {
        public Task<IEnumerable<ResolutionData>> ResolveConflicts(UpdatedAssetData data, ImportSettingsInternal importSettings)
        {
            if (importSettings.SkipImportModal)
            {
                List<ResolutionData> resolutions = new List<ResolutionData>();

                foreach (var assetDataResolutionInfo in data.Assets)
                {
                     resolutions.Add(new ResolutionData()
                    {
                        AssetData = assetDataResolutionInfo.AssetData,
                        ResolutionSelection = ResolutionSelection.Replace
                    });
                }

                foreach (var assetDataResolutionInfo in data.Dependants)
                {
                    var resolutionSelection = ResolutionSelection.Replace;
                    if (importSettings.AvoidRollingBackAssetVersion)
                    {
                        // If the user has the setting to avoid rolling back version of dependencies
                        // to a lower version and we would be rolling back, skip
                        if (assetDataResolutionInfo.CurrentVersion > assetDataResolutionInfo.AssetData.SequenceNumber)
                        {
                            resolutionSelection = ResolutionSelection.Ignore;
                        }
                    }

                    resolutions.Add(new ResolutionData()
                    {
                        AssetData = assetDataResolutionInfo.AssetData,
                        ResolutionSelection = resolutionSelection
                    });
                }
                return Task.FromResult<IEnumerable<ResolutionData>>(resolutions);
            }

            TaskCompletionSource<IEnumerable<ResolutionData>> tcs = new();

            var assetOperationManager = ServicesContainer.instance.Resolve<IAssetOperationManager>();
            assetOperationManager.PauseAllOperations();

#if UNITY_STANDALONE_OSX
            var application = ServicesContainer.instance.Resolve<IApplicationProxy>();
            application.DelayCall += () =>
            {
#endif
            ReimportWindow.CreateModalWindow(data, importSettings, resolutions =>
                {
                    assetOperationManager.ResumeAllOperations();
                    tcs.SetResult(resolutions);
                },
                () =>
                {
                    assetOperationManager.ResumeAllOperations();
                    tcs.SetResult(null);
                });
#if UNITY_STANDALONE_OSX
            };
#endif
            return tcs.Task;
        }
    }
}
