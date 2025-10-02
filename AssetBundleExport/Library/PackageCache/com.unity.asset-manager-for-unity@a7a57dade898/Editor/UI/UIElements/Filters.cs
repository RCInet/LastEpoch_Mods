using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string k_Filter = "unity-filters";
        public const string k_FilterItemButton = k_Filter + "-button";
        public const string k_FilterItemButtonCaret = k_FilterItemButton + "-caret";
        public const string k_FilterItemPopup = k_Filter + "-popup";
        public const string k_FilterSelectionSearchBar = k_FilterItemPopup + "-search-bar";
        public const string k_FilterItemSelection = k_FilterItemPopup + "-filter-selection";
        public const string k_FilterItemSelectionCheckmark = k_FilterItemSelection + "-checkmark";
        public const string k_FilterItemSelectionCheckbox = k_FilterItemSelection + "-checkbox";
        public const string k_FilterSectionLabel = k_FilterItemPopup + "-section-label";
        public const string k_FilterSectionLabelOther = k_FilterSectionLabel + "--other";
        public const string k_FilterItemNoSelection = k_FilterItemPopup + "-filter-no-selection";
        public const string k_FilterItemChipContainer = k_Filter + "-chip-container";
        public const string k_FilterItemChip = k_Filter + "-chip";
        public const string k_FilterItemChipSet = k_FilterItemChip + "--set";
        public const string k_FilterItemChipDelete = k_FilterItemChip + "-delete";
        public const string k_FilterSelectionContainer = k_FilterItemSelection + "-container";
        public const string k_FilterSelectionLabel = k_FilterItemSelection + "-label";
        public const string k_FilterSelectionNumber = k_FilterItemSelection + "-number";
        public const string k_FilterSelectionNumberField = k_FilterSelectionNumber + "-field";
        public const string k_FilterSelectionButton = k_FilterItemSelection + "-button";
        public const string k_FilterSeparatorLine = k_FilterItemPopup + "-separator-line";
    }

    class Filters : GridTool
    {
        const string k_SelfCenter = "self-center";
        const int k_ShowSearchBarThreshold = 10;

        readonly IApplicationProxy m_ApplicationProxy;
        readonly IPopupManager m_PopupManager;

        readonly Dictionary<VisualElement, BaseFilter> m_FilterPerChip = new();

        Button m_FilterButton;

        VisualElement m_ChipContainer;
        VisualElement m_CurrentChip;

        IPageFilterStrategy PageFilterStrategy => m_PageManager?.PageFilterStrategy;
        List<BaseFilter> SelectedFilters => PageFilterStrategy?.SelectedFilters ?? new List<BaseFilter>();
        protected override VisualElement Container => m_FilterButton;

        Task m_AddSelectionTask;
        CancellationTokenSource m_CancellationTokenSource;

        public Filters(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider,
            IApplicationProxy applicationProxy, IPopupManager popupManager)
            : base(pageManager, projectOrganizationProvider)
        {
            m_ApplicationProxy = applicationProxy;
            m_PopupManager = popupManager;

            AddToClassList(UssStyle.k_Filter);

            InitializeUI();
        }

        protected override void OnAttachToPanel(AttachToPanelEvent evt)
        {
            base.OnAttachToPanel(evt);

            if (PageFilterStrategy != null)
            {
                PageFilterStrategy.SavedFilterApplied += Refresh;
                PageFilterStrategy.EnableStatusChanged += OnEnableStatusChanged;
                PageFilterStrategy.FilterApplied += OnFilterApplied;
                PageFilterStrategy.FilterAdded += OnFilterAdded;
                PageFilterStrategy.FiltersCleared += Refresh;
            }

            m_PopupManager.Container.RegisterCallback<FocusOutEvent>(DeleteEmptyChip);
        }

        protected override void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            base.OnDetachFromPanel(evt);

            if (PageFilterStrategy != null)
            {
                PageFilterStrategy.SavedFilterApplied -= Refresh;
                PageFilterStrategy.EnableStatusChanged -= OnEnableStatusChanged;
                PageFilterStrategy.FilterApplied -= OnFilterApplied;
                PageFilterStrategy.FilterAdded -= OnFilterAdded;
                PageFilterStrategy.FiltersCleared -= Refresh;
            }

            m_PopupManager.Container.UnregisterCallback<FocusOutEvent>(DeleteEmptyChip);
        }

        protected override void OnActivePageChanged(IPage page)
        {
            Refresh();
            base.OnActivePageChanged(page);
        }

        protected override void InitDisplay(IPage page)
        {
            UIElementsUtils.SetDisplay(Container, true);
        }

        protected override bool IsDisplayed(IPage page)
        {
            if (page is BasePage basePage)
            {
                return basePage.DisplayFilters;
            }

            return base.IsDisplayed(page);
        }

        void Refresh()
        {
            Clear();
            m_FilterPerChip.Clear();

            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = null;

            // The first call of InitializeUI is made into the constructor
            InitializeUI();
        }

        void InitializeUI()
        {
            m_FilterButton = new Button();
            m_FilterButton.AddToClassList(UssStyle.k_FilterItemButton);

            Add(m_FilterButton);

            var label = new TextElement();
            label.text = L10n.Tr("Add Filter");
            label.AddToClassList("unity-text-element");
            m_FilterButton.Add(label);

            var caret = new VisualElement();
            caret.AddToClassList(UssStyle.k_FilterItemButtonCaret);
            m_FilterButton.Add(caret);

            m_ChipContainer = new VisualElement();
            m_ChipContainer.AddToClassList(UssStyle.k_FilterItemChipContainer);
            Add(m_ChipContainer);

            foreach (var filter in SelectedFilters)
            {
                m_ChipContainer.Add(CreateChipButton(filter));
            }

            m_FilterButton.SetEnabled(PageFilterStrategy?.IsAvailableFilters() ?? false);
            m_FilterButton.clicked += OnFilterButtonClicked;

            InitDisplay(m_PageManager.ActivePage);
        }

        void OnFilterButtonClicked()
        {
            PageFilterStrategy.Cancel();

            var availablePrimaryMetadataFilters =
                PageFilterStrategy.GetAvailablePrimaryMetadataFilters() ?? new List<BaseFilter>();
            var availableCustomMetadataFilters =
                PageFilterStrategy.GetAvailableCustomMetadataFilters() ?? new List<CustomMetadataFilter>();

            var filterSelections = new ScrollView();

            if (availablePrimaryMetadataFilters.Count + availableCustomMetadataFilters.Count >= k_ShowSearchBarThreshold)
            {
                var searchBar = new ToolbarSearchField();
                searchBar.AddToClassList(UssStyle.k_FilterSelectionSearchBar);
                searchBar.RegisterValueChangedCallback(evt =>
                {
                    filterSelections.Clear();

                    var search = evt.newValue.ToLower();
                    if (string.IsNullOrEmpty(search))
                    {
                        BuildSelections(availablePrimaryMetadataFilters, availableCustomMetadataFilters, filterSelections);
                        return;
                    }

                    var filteredPrimaryMetadataFilters = availablePrimaryMetadataFilters
                        .Where(filter => filter.DisplayName.ToLower().Contains(search.ToLower())).ToList();
                    var filteredCustomMetadataFilters = availableCustomMetadataFilters
                        .Where(filter => filter.DisplayName.ToLower().Contains(search.ToLower())).ToList();

                    BuildSelections(filteredPrimaryMetadataFilters, filteredCustomMetadataFilters, filterSelections);
                });
                m_PopupManager.Container.Add(searchBar);
            }

            m_PopupManager.Container.Add(filterSelections);

            BuildSelections(availablePrimaryMetadataFilters, availableCustomMetadataFilters, filterSelections);

            m_PopupManager.Show(m_FilterButton, PopupContainer.PopupAlignment.BottomLeft);

            AnalyticsSender.SendEvent(new FilterDropdownEvent(m_PageManager.ActivePage.Title));
        }

        void BuildSelections(List<BaseFilter> availablePrimaryMetadataFilters, List<CustomMetadataFilter> availableCustomMetadataFilters, ScrollView filterSelections)
        {
            var showTitle = PageFilterStrategy.HasCustomMetadataFilters;

            if (showTitle && availablePrimaryMetadataFilters.Any())
            {
                var primaryLabel = new Label(L10n.Tr(Constants.PrimaryMetadata));
                primaryLabel.AddToClassList(UssStyle.k_FilterSectionLabel);
                filterSelections.Add(primaryLabel);
            }

            AddFilterSelections(availablePrimaryMetadataFilters, filterSelections);

            if (showTitle && availableCustomMetadataFilters.Any())
            {
                var customLabel = new Label(L10n.Tr(Constants.CustomMetadata));
                customLabel.AddToClassList(UssStyle.k_FilterSectionLabel);
                if (availablePrimaryMetadataFilters.Any())
                {
                    customLabel.AddToClassList(UssStyle.k_FilterSectionLabelOther);
                }
                filterSelections.Add(customLabel);
            }

            AddFilterSelections(availableCustomMetadataFilters, filterSelections);
        }

        void AddFilterSelections(IEnumerable<BaseFilter> availableFilters, ScrollView filterSelections)
        {
            foreach (var filter in availableFilters)
            {
                var filterSelection = new TextElement();
                filterSelection.AddToClassList(UssStyle.k_FilterItemSelection);
                filterSelection.text = filter.DisplayName;
                filterSelection.RegisterCallback<ClickEvent>(evt =>
                {
                    evt.StopPropagation();
                    AddFilter(filter);
                });

                filterSelections.Add(filterSelection);
            }
        }

        Button CreateChipButton(BaseFilter filter)
        {
            var chip = new Button();
            chip.AddToClassList(UssStyle.k_FilterItemChip);
            chip.clicked += () => OnChipClicked(chip, filter);
            m_ChipContainer.Add(chip);

            var label = new TextElement();
            label.name = "label";
            label.text = filter.DisplaySelectedFilters();
            if (filter.SelectedFilters != null && filter.SelectedFilters.Any())
            {
                chip.AddToClassList(UssStyle.k_FilterItemChipSet);
            }

            chip.Add(label);

            var delete = new Image();
            delete.AddToClassList(UssStyle.k_FilterItemChipDelete);
            delete.AddManipulator(new Clickable(() => OnChipDeleteClicked(chip, filter)));
            chip.Add(delete);

            m_FilterPerChip.TryAdd(chip, filter);

            return chip;
        }

        void AddFilter(BaseFilter filter)
        {
            m_PopupManager.Hide();

            PageFilterStrategy.AddFilter(filter, true);
            m_FilterButton.SetEnabled(PageFilterStrategy.IsAvailableFilters());
        }

        void WaitUntilChipIsPositioned(Button chip, BaseFilter filter)
        {
            if (UnityEngine.Mathf.Approximately(chip.resolvedStyle.top, 0))
            {
                m_ApplicationProxy.DelayCall += () => WaitUntilChipIsPositioned(chip, filter);
                return;
            }

            OnChipClicked(chip, filter);
        }

        void OnChipClicked(Button chip, BaseFilter filter)
        {
            m_CurrentChip = chip;

            m_PopupManager.Show(chip, PopupContainer.PopupAlignment.BottomLeft);

            var loadingLabel = new TextElement();
            loadingLabel.text = L10n.Tr(Constants.LoadingText);
            loadingLabel.AddToClassList(k_SelfCenter);
            m_PopupManager.Container.Add(loadingLabel);

            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = new CancellationTokenSource();
            m_AddSelectionTask = AddFilterSelectionItems(chip, filter, m_CancellationTokenSource.Token);

            TaskUtils.TrackException(m_AddSelectionTask);
        }

        void OnChipDeleteClicked(Button chip, BaseFilter filter)
        {
            m_FilterPerChip.Remove(chip);
            chip.RemoveFromHierarchy();
            PageFilterStrategy.RemoveFilter(filter);
            m_FilterButton.SetEnabled(PageFilterStrategy.IsAvailableFilters());

            ApplyFilter(filter, null);

            m_PopupManager.Hide();
        }

        async Task AddFilterSelectionItems(Button chip, BaseFilter filter, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            switch (filter.SelectionType)
            {
                case FilterSelectionType.SingleSelection:
                    await AddSingleSelectionItems(chip, filter, cancellationToken);
                    break;
                case FilterSelectionType.MultiSelection:
                    await AddMultiSelectionItems(chip, filter, cancellationToken);
                    break;
                case FilterSelectionType.Number:
                    AddNumberSelection(chip, filter);
                    break;
                case FilterSelectionType.NumberRange:
                    AddRangeNumberSelection(chip, filter);
                    break;
                case FilterSelectionType.Timestamp:
                    AddRangeTimestampSelection(chip, filter);
                    break;
                case FilterSelectionType.Text:
                    AddTextSelection(chip, filter);
                    break;
                case FilterSelectionType.Url:
                    AddUrlSelection(chip, filter);
                    break;
                default:
                    Utilities.DevLogError($"{filter.SelectionType} is not supported");
                    break;
            }
        }

        async Task AddSingleSelectionItems(Button chip, BaseFilter filter, CancellationToken cancellationToken)
        {
            var selections = await filter.GetSelections(true);
            if (selections == null)
                return;

            if (cancellationToken.IsCancellationRequested)
                return;

            m_PopupManager.Clear();

            if (selections.Any())
            {
                var scrollView = new ScrollView();
                m_PopupManager.Container.Add(scrollView);

                foreach (var selection in selections)
                {
                    var filterSelection = new VisualElement();
                    filterSelection.AddToClassList(UssStyle.k_FilterItemSelection);
                    filterSelection.style.paddingLeft = 0;

                    var checkbox = new VisualElement();
                    checkbox.AddToClassList(UssStyle.k_FilterItemSelectionCheckbox);
                    filterSelection.Add(checkbox);

                    var checkmark = new Image();
                    checkmark.AddToClassList(UssStyle.k_FilterItemSelectionCheckmark);
                    filterSelection.Add(checkmark);

                    var label = new TextElement();
                    label.text = selection.Text;
                    label.tooltip = selection.Tooltip;
                    filterSelection.Add(label);

                    checkmark.visible = filter.SelectedFilters?.Any(s => s == selection.Text) ?? false;

                    filterSelection.RegisterCallback<ClickEvent>(evt =>
                    {
                        evt.StopPropagation();

                        chip.AddToClassList(UssStyle.k_FilterItemChipSet);
                        m_PopupManager.Hide();

                        ApplyFilter(filter, new List<string> {selection.Text});
                    });

                    scrollView.Add(filterSelection);
                }
            }
            else
            {
                var noSelection = new TextElement();
                noSelection.AddToClassList(UssStyle.k_FilterItemNoSelection);
                noSelection.text = L10n.Tr(Constants.NoSelectionsText);
                m_PopupManager.Container.Add(noSelection);
            }
        }

        Button AddButtons(Button chip, BaseFilter filter, Func<List<string>> applySelection, Action clearAction)
        {
            var line = new VisualElement();
            line.AddToClassList(UssStyle.k_FilterSeparatorLine);
            m_PopupManager.Container.Add(line);

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList(UssStyle.k_FilterSelectionContainer);
            m_PopupManager.Container.Add(buttonContainer);

            var clearButton = new Button();
            clearButton.AddToClassList(UssStyle.k_FilterSelectionButton);
            clearButton.text = L10n.Tr(Constants.Clear);
            clearButton.clicked += clearAction;
            buttonContainer.Add(clearButton);

            var applyButton = new Button();
            applyButton.AddToClassList(UssStyle.k_FilterSelectionButton);
            applyButton.text = L10n.Tr(Constants.Apply);
            applyButton.clicked += () =>
            {
                var selectedFilters = applySelection();
                if(selectedFilters == null || !selectedFilters.Any())
                {
                    return;
                }

                chip.AddToClassList(UssStyle.k_FilterItemChipSet);
                m_PopupManager.Hide();

                ApplyFilter(filter, selectedFilters);
            };
            buttonContainer.Add(applyButton);

            return applyButton;
        }

        async Task AddMultiSelectionItems(Button chip, BaseFilter filter, CancellationToken cancellationToken)
        {
            var selections = await filter.GetSelections(true);
            if (selections == null)
                return;

            if (cancellationToken.IsCancellationRequested)
                return;

            m_PopupManager.Clear();

            if (selections.Any())
            {
                var scrollView = new ScrollView();
                m_PopupManager.Container.Add(scrollView);

                var selectedFilters = filter.SelectedFilters?.ToList() ?? new List<string>();
                var checkmarks = new List<Image>();

                Button applyButton = null;
                applyButton = AddButtons(chip, filter, () => selectedFilters,
                    () => {
                        selectedFilters.Clear();
                        foreach (var checkmark in checkmarks)
                        {
                            checkmark.visible = false;
                        }

                        applyButton?.SetEnabled(false);
                    });
                applyButton.SetEnabled(selectedFilters.Any());

                foreach (var selection in selections)
                {
                    var filterSelection = new VisualElement();
                    filterSelection.AddToClassList(UssStyle.k_FilterItemSelection);
                    filterSelection.style.paddingLeft = 0;

                    var checkbox = new VisualElement();
                    checkbox.AddToClassList(UssStyle.k_FilterItemSelectionCheckbox);
                    filterSelection.Add(checkbox);

                    var checkmark = new Image();
                    checkmark.AddToClassList(UssStyle.k_FilterItemSelectionCheckmark);
                    checkmarks.Add(checkmark);
                    filterSelection.Add(checkmark);

                    var label = new TextElement();
                    label.text = selection.Text;
                    label.tooltip = selection.Tooltip;
                    filterSelection.Add(label);

                    checkmark.visible = filter.SelectedFilters?.Any(s => s == selection.Text) ?? false;

                    filterSelection.RegisterCallback<ClickEvent>(evt =>
                    {
                        evt.StopPropagation();

                        if (selectedFilters.Contains(selection.Text))
                        {
                            selectedFilters.Remove(selection.Text);
                            applyButton.SetEnabled(selectedFilters.Any());
                        }
                        else
                        {
                            selectedFilters.Add(selection.Text);
                            applyButton.SetEnabled(true);
                        }

                        checkmark.visible = selectedFilters.Contains(selection.Text);
                    });

                    scrollView.Add(filterSelection);
                }
            }
            else
            {
                var noSelection = new TextElement();
                noSelection.AddToClassList(UssStyle.k_FilterItemNoSelection);
                noSelection.text = L10n.Tr(Constants.NoSelectionsText);
                m_PopupManager.Container.Add(noSelection);
            }
        }

        void AddNumberSelection(Button chip, BaseFilter filter)
        {
            m_PopupManager.Clear();

            var numberFilter = (NumberMetadataFilter)filter;
            if(numberFilter == null)
                return;

            var valueField = new DoubleField(L10n.Tr(Constants.EnterNumberText))
            {
                value = numberFilter.Value
            };

            valueField.AddToClassList(UssStyle.k_FilterSelectionNumberField);
            valueField.isDelayed = true;
            m_PopupManager.Container.Add(valueField);

            AddButtons(chip, filter, () => new List<string>{valueField.value.ToString()},
                () =>
                {
                    valueField.value = 0;
                });
        }

        void AddRangeNumberSelection(Button chip, BaseFilter filter)
        {
            m_PopupManager.Clear();

            var numberFilter = (NumberRangeMetadataFilter)filter;
            if(numberFilter == null)
                return;

            var fromField = new DoubleField(L10n.Tr(Constants.FromText))
            {
                value = numberFilter.FromValue
            };

            var toField = new DoubleField(L10n.Tr(Constants.ToText))
            {
                value = numberFilter.ToValue
            };

            fromField.AddToClassList(UssStyle.k_FilterSelectionNumberField);
            fromField.isDelayed = true;
            fromField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue > toField.value)
                {
                    toField.value = evt.newValue;
                }
            });
            m_PopupManager.Container.Add(fromField);

            toField.AddToClassList(UssStyle.k_FilterSelectionNumberField);
            toField.isDelayed = true;
            toField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue < fromField.value)
                {
                    toField.value = evt.previousValue;
                }
            });
            m_PopupManager.Container.Add(toField);

            AddButtons(chip, filter, () => new List<string>{fromField.value.ToString(), toField.value.ToString()},
                () =>
                {
                    fromField.value = 0;
                    toField.value = 0;
                });
        }

        void AddRangeTimestampSelection(Button chip, BaseFilter filter)
        {
            m_PopupManager.Clear();

            var timestampFilter = (TimestampMetadataFilter)filter;

            if(timestampFilter == null)
                return;

            var fromField = new TimestampPicker(timestampFilter.FromValue != DateTime.MinValue ? timestampFilter.FromValue : DateTime.Today , false);
            var toField = new TimestampPicker(timestampFilter.ToValue != DateTime.MinValue ? timestampFilter.ToValue : EndOfDay(DateTime.Today), false);

            fromField.ValueChanged += dateTime =>
            {
                if(DateTime.Compare(dateTime, toField.Timestamp) > 0)
                {
                    toField.Timestamp = EndOfDay(dateTime);
                }
            };

            toField.ValueChanged += dateTime =>
            {
                if (DateTime.Compare(dateTime, fromField.Timestamp) < 0)
                {
                    toField.Timestamp = EndOfDay(fromField.Timestamp);
                }
            };

            var fromContainer = new VisualElement();
            fromContainer.AddToClassList(UssStyle.k_FilterSelectionContainer);
            m_PopupManager.Container.Add(fromContainer);

            var fromLabel = new Label(L10n.Tr(Constants.FromText));
            fromLabel.AddToClassList(UssStyle.k_FilterSelectionLabel);
            fromContainer.Add(fromLabel);
            fromContainer.Add(fromField);

            var toContainer = new VisualElement();
            toContainer.AddToClassList(UssStyle.k_FilterSelectionContainer);
            m_PopupManager.Container.Add(toContainer);

            var toLabel = new Label(L10n.Tr(Constants.ToText));
            toLabel.AddToClassList(UssStyle.k_FilterSelectionLabel);
            toContainer.Add(toLabel);
            toContainer.Add(toField);

            AddButtons(chip, filter, () => new List<string>{fromField.Timestamp.ToString(), toField.Timestamp.ToString()},
                () =>
                {
                    fromField.Timestamp = DateTime.Today;
                    toField.Timestamp = EndOfDay(DateTime.Today);
                });
        }

        void AddTextSelection(Button chip, BaseFilter filter)
        {
            m_PopupManager.Clear();

            var container = new VisualElement();
            container.AddToClassList(UssStyle.k_FilterSelectionContainer);
            m_PopupManager.Container.Add(container);

            var label = new Label(L10n.Tr(Constants.EnterText));
            label.AddToClassList(UssStyle.k_FilterSelectionLabel);
            container.Add(label);

            var textField = new TextField();
            textField.value = filter.SelectedFilters?.FirstOrDefault() ?? string.Empty;
            textField.AddToClassList(UssStyle.k_FilterSelectionNumberField);
            container.Add(textField);

            AddButtons(chip, filter, () => new List<string>{textField.value},
                () =>
                {
                    textField.value = string.Empty;
                });
        }

        void AddUrlSelection(Button chip, BaseFilter filter)
        {
            m_PopupManager.Clear();

            var container = new VisualElement();
            container.AddToClassList(UssStyle.k_FilterSelectionContainer);
            m_PopupManager.Container.Add(container);

            var label = new Label(L10n.Tr(Constants.EnterUrlText));
            label.AddToClassList(UssStyle.k_FilterSelectionLabel);
            container.Add(label);

            var textField = new TextField();
            textField.value = filter.SelectedFilters?.FirstOrDefault() ?? string.Empty;
            textField.AddToClassList(UssStyle.k_FilterSelectionNumberField);
            container.Add(textField);

            AddButtons(chip, filter, () => new List<string>{textField.value},
                () =>
                {
                    textField.value = string.Empty;
                });
        }

        void ApplyFilter(BaseFilter filter, List<string> selectedFilters)
        {
            if (selectedFilters != null)
            {
                AnalyticsSender.SendEvent(new FilterSearchEvent(filter.DisplayName, selectedFilters, m_PageManager.ActivePage.Title));
                m_PageManager.ActivePage.LoadingStatusChanged += OnActivePageLoadingStatusChanged;
            }

            PageFilterStrategy.ApplyFilter(filter, selectedFilters);
        }

        void DeleteEmptyChip(FocusOutEvent evt)
        {
            if (evt.relatedTarget != null && (evt.relatedTarget == m_PopupManager.Container ||
                    m_PopupManager.Container.Contains((VisualElement)evt.relatedTarget)))
                return;

            if (m_CurrentChip != null)
            {
                m_CancellationTokenSource?.Cancel();
                m_CancellationTokenSource?.Dispose();
                m_CancellationTokenSource = null;

                if (m_FilterPerChip.TryGetValue(m_CurrentChip, out var filter) &&
                    (filter.SelectedFilters == null || !filter.SelectedFilters.Any()))
                {
                    m_FilterPerChip.Remove(m_CurrentChip);
                    m_CurrentChip.RemoveFromHierarchy();
                    PageFilterStrategy.RemoveFilter(filter);
                    m_FilterButton.SetEnabled(PageFilterStrategy.IsAvailableFilters());
                }

                m_CurrentChip = null;
            }
        }

        void OnActivePageLoadingStatusChanged(bool isLoading)
        {
            if (!isLoading)
            {
                m_PageManager.ActivePage.LoadingStatusChanged -= OnActivePageLoadingStatusChanged;
                var filters = new List<FilterSearchResultEvent.FilterData>();
                foreach (var filter in SelectedFilters)
                {
                    filters.Add(new FilterSearchResultEvent.FilterData
                        { FilterName = filter.DisplayName, FilterValue = filter.SelectedFilters != null ? string.Join(",", filter.SelectedFilters) : null });
                }

                AnalyticsSender.SendEvent(new FilterSearchResultEvent(filters, m_PageManager.ActivePage.AssetList.Count, m_PageManager.ActivePage.Title));
            }
        }

        void OnEnableStatusChanged(bool isEnabled)
        {
            m_FilterButton?.SetEnabled(isEnabled);
        }

        void OnFilterAdded(BaseFilter filter, bool showSelection)
        {
            var chip = CreateChipButton(filter);

            if (showSelection)
            {
                WaitUntilChipIsPositioned(chip, filter);
            }
        }

        void OnFilterApplied(BaseFilter filter)
        {
            foreach (var keyValuePair in m_FilterPerChip)
            {
                if (keyValuePair.Value.GetType() == filter.GetType())
                {
                    keyValuePair.Key.Q<TextElement>("label").text = filter.DisplaySelectedFilters();
                }
            }
        }

        static DateTime EndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Local);
        }
    }
}
