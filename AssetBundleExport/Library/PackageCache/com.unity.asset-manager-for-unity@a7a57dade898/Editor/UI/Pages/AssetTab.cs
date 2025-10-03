using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string LoadingLabel = "details-page-loading-label";
    }

    abstract class AssetTab : IPageComponent
    {
        public event CreateProjectChip CreateProjectChip;
        public event CreateUserChip CreateUserChip;
        public event Action<IEnumerable<string>> ApplyFilter;

        public abstract AssetDetailsPageTabs.TabType Type { get; }
        public abstract bool IsFooterVisible { get; }
        public abstract VisualElement Root { get; }
        public abstract bool EnabledWhenDisconnected { get; }

        protected static void AddLoadingText(VisualElement container)
        {
            var loadingLabel = new Label
            {
                text = L10n.Tr(Constants.LoadingText)
            };
            loadingLabel.AddToClassList(UssStyle.LoadingLabel);

            container.Add(loadingLabel);
        }

        protected static void AddLabel(VisualElement container, string title, string name = null)
        {
            var entry = new DetailsPageEntry(title)
            {
                name = name
            };
            container.Add(entry);
        }

        protected static void AddSpace(VisualElement container, int height = 10)
        {
            var space = new VisualElement();
            space.style.height = height;
            container.Add(space);
        }

        protected static void AddText(VisualElement container, string title, string details, bool isSelectable = false, string name = null)
        {
            if (string.IsNullOrEmpty(details))
            {
                return;
            }

            var entry = new DetailsPageEntry(title, details, isSelectable)
            {
                name = name
            };
            container.Add(entry);
        }

        protected static void AddText(VisualElement container, string title, string details, IEnumerable<string> classNames, string name = null)
        {
            if (string.IsNullOrEmpty(details))
                return;

            var entry = new DetailsPageEntry(title, details)
            {
                name = name
            };

            if (classNames != null)
            {
                foreach (var className in classNames)
                {
                    entry.AddToClassList(className);
                }
            }

            container.Add(entry);
        }

        protected static void AddAssetIdentifier(VisualElement entriesContainer, string title, AssetIdentifier identifier)
        {
            if (identifier.IsLocal())
                return;

            AddText(entriesContainer, title, identifier.AssetId, isSelectable:true);
        }

        protected void AddUser(VisualElement container, string title, string details, Type searchFilterType, string name = null)
        {
            if (string.IsNullOrEmpty(details))
                return;

            var entry = new DetailsPageEntry(title)
            {
                name = name
            };
            container.Add(entry);

            var chipContainer = entry.AddChipContainer();
            CreateUserChip?.AddUserChip(chipContainer, details, searchFilterType);
        }

        protected void AddProjectChips(VisualElement container, string title, string[] projectIds, string name = null)
        {
            var entry = new DetailsPageEntry(title)
            {
                name = name
            };
            container.Add(entry);

            var chipContainer = entry.AddChipContainer();
            CreateProjectChip?.AddProjectChip(chipContainer, projectIds);
        }

        void AddChips(VisualElement container, string title, IEnumerable<string> chips,
            Func<string, Chip> chipCreator, string name = null)
        {
            var enumerable = chips as string[] ?? chips.ToArray();
            if (!enumerable.Any())
            {
                return;
            }

            var entry = new DetailsPageEntry(title)
            {
                name = name
            };
            container.Add(entry);

            var chipsContainer = entry.AddChipContainer();
            chipsContainer.AddToClassList(UssStyle.FlexWrap);

            foreach (var chipText in enumerable)
            {
                var chip = chipCreator?.Invoke(chipText);
                chipsContainer.Add(chip);
            }
        }

        protected void AddTagChips(VisualElement container, string title, IEnumerable<string> chips,
            string name = null)
        {
            AddChips(container, title,chips, chipText =>
            {
                var tagChip = new TagChip(chipText);
                tagChip.TagChipPointerUpAction += tagText =>
                {
                    var words = tagText.Split(' ').Where(w => !string.IsNullOrEmpty(w));
                    ApplyFilter?.Invoke(words);
                };

                return tagChip;
            }, name);
        }

        protected void AddSelectionChips(VisualElement container, string title, IEnumerable<string> chips, bool isSelectable = false,
            string name = null)
        {
            AddChips(container, title,chips, chipText => new Chip(chipText, isSelectable),  name);
        }

        protected void AddToggle(VisualElement container, string title, bool toggleValue, string name = null)
        {
            var entry = new DetailsPageEntry(title)
            {
                name = name
            };
            container.Add(entry);

            var toggle = new Toggle
            {
                value = toggleValue
            };
            toggle.SetEnabled(false);

            entry.Add(toggle);
        }

        public abstract void OnSelection(BaseAssetData assetData);
        public abstract void RefreshUI(BaseAssetData assetData, bool isLoading = false);
        public abstract void RefreshButtons(UIEnabledStates enabled, BaseAssetData assetData, BaseOperation operationInProgress);
    }
}
