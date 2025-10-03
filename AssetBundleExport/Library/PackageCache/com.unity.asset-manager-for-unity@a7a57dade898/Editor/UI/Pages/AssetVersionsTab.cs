using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string DetailsPageContentContainer = "details-page-content-container";
        public const string AssetVersionDetailsFoldout = "asset-version-details-foldout";
        public const string AssetVersionLabelContainer = "asset-version-label-container";
        public const string AssetVersionLabel = "asset-version-label";
        public const string AssetVersionLabel_Filled = "asset-version-label--filled";
        public const string AssetVersionLabel_Imported = "asset-version-label--imported";
        public const string UnityFoldoutInput = "unity-foldout__input";
    }

    class AssetVersionsTab : AssetTab
    {
        struct LoadingTask
        {
            public AssetIdentifier Target { get; }
            public Action RefreshUI { get; set; }
            public Action RefreshButtons { get; set; }
            public bool IsRunning { get; }

            public LoadingTask(AssetIdentifier target, Func<Task> task)
            {
                Target = target;
                RefreshUI = null;
                RefreshButtons = null;

                IsRunning = task != null;
                task?.Invoke();
            }
        }

        const string k_FoldoutLabelsContainer = "foldout-labels-container";
        const string k_ImportedTagContainer = "imported-tag";
        const string k_PreferencesProjectId = "selected-project-id";

        static readonly string k_NoChangelogProvided = $"<i>{L10n.Tr(Constants.NoChangeLogText)}</i>";

        readonly IDialogManager m_DialogManager;
        readonly IUIPreferences m_UIPreferences;
        readonly List<string> m_ImportedVersions = new List<string>();

        string m_CurrentProjectId;
        LoadingTask m_LoadingTask;
        CancellationTokenSource m_LoadingTaskCancellationTokenSource;

        public override AssetDetailsPageTabs.TabType Type => AssetDetailsPageTabs.TabType.Versions;
        public override bool IsFooterVisible => false;
        public override bool EnabledWhenDisconnected => false;
        public override VisualElement Root { get; }

        public event Action<ImportTrigger, string, IEnumerable<BaseAssetData>> ImportAsset;

        public AssetVersionsTab(VisualElement visualElement, IDialogManager dialogManager)
        {
            var root = new VisualElement();
            root.AddToClassList(UssStyle.DetailsPageContentContainer);
            visualElement.Add(root);

            m_DialogManager = dialogManager;
            m_UIPreferences = ServicesContainer.instance.Resolve<IUIPreferences>();

            m_CurrentProjectId = m_UIPreferences.GetString(k_PreferencesProjectId, string.Empty);

            Root = root;
        }

        public override void OnSelection(BaseAssetData assetData)
        {
            if (m_CurrentProjectId != assetData.Identifier.ProjectId)
            {
                m_UIPreferences.RemoveAll("foldout:");
                m_CurrentProjectId = assetData.Identifier.ProjectId;
                m_UIPreferences.SetString(k_PreferencesProjectId, m_CurrentProjectId);
            }

            // Have the loaded version expanded by default if the foldout is not already set
            if (!m_UIPreferences.Contains($"foldout:{assetData.Identifier.AssetId}"))
            {
                m_UIPreferences.SetBool($"foldout:{assetData.Identifier.AssetId}", true);
                m_UIPreferences.SetBool(GetFoldoutKey(assetData), true);
            }

            ClearLoadingCancellationTokenSource();
            m_LoadingTaskCancellationTokenSource = new CancellationTokenSource();
            m_LoadingTask = new LoadingTask(assetData.Identifier,
                () => LoadVersions(assetData, m_LoadingTaskCancellationTokenSource.Token));
        }

        public override void RefreshUI(BaseAssetData assetData, bool isLoading = false)
        {
            Root.Clear();

            TryDisplayLoadingMessage(assetData);

            if (m_LoadingTask.IsRunning)
            {
                m_LoadingTask.RefreshUI = () => RefreshUI(assetData, isLoading);
                return;
            }

            foreach (var data in assetData.Versions)
            {
                var foldout = CreateFoldout(data);

                if (data.Labels != null)
                {
                    foreach (var label in data.Labels)
                    {
                        AddText(foldout.parent.Q(k_FoldoutLabelsContainer), null, label.Name, new[] {UssStyle.AssetVersionLabel, UssStyle.AssetVersionLabel_Filled});
                    }
                }

                if (data.SequenceNumber > 0)
                {
                    AddLabel(foldout, Constants.ChangeLogText);
                    AddText(foldout, null, string.IsNullOrEmpty(data.Changelog) ? k_NoChangelogProvided : data.Changelog);
                }

                if (data.ParentSequenceNumber > 0)
                {
                    AddText(foldout, Constants.CreatedFromText, L10n.Tr(Constants.VersionText) + " " + data.ParentSequenceNumber);
                }

                AddUser(foldout, Constants.CreatedByText, data.CreatedBy, typeof(CreatedByFilter));
                AddText(foldout, Constants.DateText, data.Updated?.ToLocalTime().ToString("G"));
                AddText(foldout, Constants.StatusText, data.Status);

                var importButton = new ImportButton(m_DialogManager);
                foldout.Add(importButton);
                importButton.RegisterCallback(importLocation => BeginImport(importLocation, data));
            }
        }

        public override void RefreshButtons(UIEnabledStates enabled, BaseAssetData assetData, BaseOperation operationInProgress)
        {
            if (m_LoadingTask.IsRunning)
            {
                m_LoadingTask.RefreshButtons = () => RefreshButtons(enabled, assetData, operationInProgress);
                return;
            }

            if (operationInProgress is { Status: OperationStatus.Paused })
            {
                return;
            }
            
            m_ImportedVersions.Clear();

            var assetOperation = operationInProgress as AssetDataOperation;
            var isEnabled = enabled.IsImportAvailable();

            foreach (var identifier in assetData.Versions.Select(x => x.Identifier))
            {
                var foldoutContainer = Root.Q(identifier.Version);

                if (foldoutContainer == null)
                    continue;

                var versionOperation = assetOperation?.Identifier.Version == identifier.Version ? operationInProgress : null;
                var inProject = enabled.HasFlag(UIEnabledStates.InProject) && assetData.Identifier.Equals(identifier);

                if (inProject)
                {
                    m_ImportedVersions.Add(identifier.Version);
                }

                RefreshImportedChip(foldoutContainer, inProject && !enabled.HasFlag(UIEnabledStates.IsImporting));

                // We can't rely on the preview status as it may not yet be loaded.
                var button = foldoutContainer.Q<ImportButton>();
                button.text = AssetDetailsPageExtensions.GetImportButtonLabel(versionOperation, inProject ? AssetDataStatus.Imported : null);
                button.tooltip = AssetDetailsPageExtensions.GetImportButtonTooltip(versionOperation, enabled);
                button.SetEnabled(isEnabled);
            }
        }

        void ClearLoadingCancellationTokenSource()
        {
            if (m_LoadingTaskCancellationTokenSource != null)
            {
                m_LoadingTaskCancellationTokenSource.Cancel();
                m_LoadingTaskCancellationTokenSource.Dispose();
            }

            m_LoadingTaskCancellationTokenSource = null;
        }

        async Task LoadVersions(BaseAssetData assetData, CancellationToken cancellationToken)
        {
            await assetData.RefreshVersionsAsync(CancellationToken.None);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Get a handle on the loading task
            var loadingTask = m_LoadingTask;

            // Clean up
            ClearLoadingCancellationTokenSource();
            m_LoadingTask = new LoadingTask();

            // Trigger callbacks
            loadingTask.RefreshUI?.Invoke();
            loadingTask.RefreshButtons?.Invoke();
        }

        void TryDisplayLoadingMessage(BaseAssetData assetData)
        {
            if (m_LoadingTask.IsRunning)
            {
                AddLoadingText(Root);
            }
            else if (!assetData.Versions.Any())
            {
                AddText(Root, null, $"<i>{L10n.Tr(Constants.StatusErrorText)}</i>");
            }
        }

        Foldout CreateFoldout(BaseAssetData assetData)
        {
            var foldoutContainer = new VisualElement
            {
                name = assetData.Identifier.Version
            };
            Root.Add(foldoutContainer);

            var title = L10n.Tr(Constants.VersionText) + " " + assetData.SequenceNumber;
            if (assetData.SequenceNumber <= 0)
            {
                title = L10n.Tr(Constants.PendingVersionText);

                if (assetData.ParentSequenceNumber > 0)
                {
                    title += $" ({L10n.Tr(Constants.FromVersionText)} {assetData.ParentSequenceNumber})";
                }
            }

            var key = GetFoldoutKey(assetData);

            var foldout = new Foldout
            {
                text = title,
                value = m_UIPreferences.GetBool(key, false)
            };
            foldout.AddToClassList(UssStyle.AssetVersionDetailsFoldout);
            foldout.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    m_UIPreferences.SetBool(key, true);
                }
                else
                {
                    m_UIPreferences.Remove(key);
                }
            });

            foldoutContainer.Add(foldout);

            var labelsContainer = new VisualElement
            {
                name = k_FoldoutLabelsContainer
            };
            labelsContainer.AddToClassList(UssStyle.AssetVersionLabelContainer);
            var foldoutLabel = foldout.Q(null, UssStyle.UnityFoldoutInput);
            foldoutLabel.Add(labelsContainer);

            return foldout;
        }

        void BeginImport(string importLocation, BaseAssetData assetData)
        {
            var trigger = m_ImportedVersions.Contains(assetData.Identifier.Version) ? ImportTrigger.ReimportVersion : ImportTrigger.ImportVersion;
            ImportAsset?.Invoke(trigger, importLocation, new[] {assetData});
        }

        static void RefreshImportedChip(VisualElement foldoutContainer, bool isChipEnabled)
        {
            var importedTag = foldoutContainer.Q(k_ImportedTagContainer);

            if (isChipEnabled)
            {
                if (importedTag == null)
                {
                    AddText(foldoutContainer.Q(k_FoldoutLabelsContainer), null, Constants.ImportedTagText,
                        new[] {UssStyle.AssetVersionLabel, UssStyle.AssetVersionLabel_Imported}, k_ImportedTagContainer);
                }
            }
            else
            {
                importedTag?.RemoveFromHierarchy();
            }
        }

        static string GetFoldoutKey(BaseAssetData assetData)
        {
            return $"foldout:{assetData.Identifier.AssetId}_{assetData.Identifier.Version}";
        }
    }
}
