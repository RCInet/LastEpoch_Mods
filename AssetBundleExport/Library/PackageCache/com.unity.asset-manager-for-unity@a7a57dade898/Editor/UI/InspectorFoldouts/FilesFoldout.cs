using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class FilesFoldout : ItemFoldout<BaseAssetDataFile, DetailsPageFileItem>
    {
        readonly IAssetDatabaseProxy m_AssetDatabaseProxy;

        class FileItem
        {
            readonly BaseAssetData m_AssetData;
            readonly BaseAssetDataFile m_AssetDataFile;
            
            public string Filename => m_AssetDataFile.Path;
            public string Guid { get; }
            public bool Uploaded => m_AssetDataFile.Available;
            public bool CanRemove => m_AssetData.CanRemovedFile(m_AssetDataFile);

            public FileItem(BaseAssetData assetData, BaseAssetDataFile assetDataFile)
            {
                m_AssetDataFile = assetDataFile;
                m_AssetData = assetData;

                var guid = assetDataFile.Guid;
                if (string.IsNullOrEmpty(guid))
                {
                    var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();
                    guid = assetDataManager.GetImportedFileGuid(assetData?.Identifier, assetDataFile.Path);
                }

                Guid = guid;
            }
            
            public void Remove()
            {
                m_AssetData.RemoveFile(m_AssetDataFile);
            }
        }

        readonly List<FileItem> m_FilesList = new();

        public FilesFoldout(VisualElement parent, string foldoutTitle, bool isSourceControlled, IAssetDatabaseProxy assetDatabaseProxy)
            : base(parent, foldoutTitle, $"files-foldout-{GetSuffix(foldoutTitle)}", $"files-list-{GetSuffix(foldoutTitle)}", "details-files-foldout", "details-files-list")
        {
            m_AssetDatabaseProxy = assetDatabaseProxy;
            SelectionChanged += TryPingItem;

            var uvcsChip = new Chip("VCS");
            uvcsChip.AddToClassList("details-files-foldout-uvcs-chip");
            uvcsChip.tooltip = L10n.Tr(Constants.VCSChipTooltip);
            var icon = new VisualElement();
            icon.AddToClassList("details-files-foldout-uvcs-chip-icon");
            uvcsChip.Add(icon);
            m_FoldoutToggle?.Add(uvcsChip);

            UIElementsUtils.SetDisplay(uvcsChip, isSourceControlled);
        }

        public override void Clear()
        {
            base.Clear();
            m_FilesList.Clear();
        }

        protected override IList PrepareListItem(BaseAssetData assetData, IEnumerable<BaseAssetDataFile> items)
        {
            m_FilesList.Clear();

            foreach (var assetDataFile in items.OrderBy(f => f.Path))
            {
                if (AssetDataDependencyHelper.IsASystemFile(assetDataFile.Path))
                    continue;

                m_FilesList.Add(new FileItem(assetData, assetDataFile));
            }

            return m_FilesList;
        }

        protected override DetailsPageFileItem MakeItem()
        {
            return new DetailsPageFileItem(m_AssetDatabaseProxy);
        }

        protected override void BindItem(DetailsPageFileItem element, int index)
        {
            var fileItem = m_FilesList[index];

            var enabled = !MetafilesHelper.IsOrphanMetafile(fileItem.Filename, m_FilesList.Select(f => f.Filename).ToList());

            element.Refresh(fileItem.Filename, fileItem.Guid, enabled, fileItem.Uploaded, fileItem.CanRemove);
            element.RemoveClicked = () =>
            {
                fileItem.Remove();
            };
        }

        void TryPingItem(IEnumerable<object> items)
        {
            var fileItems = items.OfType<FileItem>();
            var firstPingableItem = fileItems.FirstOrDefault(fileItem => fileItem.Guid != null);

            if (firstPingableItem != null)
            {
                m_AssetDatabaseProxy.PingAssetByGuid(firstPingableItem.Guid);
            }
        }

        static string GetSuffix(string title)
        {
            return title.ToLower().Replace(' ', '-');
        }
    }
}
