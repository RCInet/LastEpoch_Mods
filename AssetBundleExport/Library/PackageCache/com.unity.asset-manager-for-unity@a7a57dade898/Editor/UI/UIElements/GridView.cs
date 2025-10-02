using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class GridView : BindableElement, ISerializationCallbackReceiver
    {
        internal enum RefreshRowsType
        {
            ClearGrid,
            RebindGrid,
            ResizeGridWidth,
            LoadMoreGridItems
        }

        // Item height/widths are used to calculate the # of rows/columns
        const int k_DefaultItemHeight = 150;
        const int k_DefaultItemWidth = 131; // 125 + 6 margin
        const int k_ExtraVisibleRows = 2;
        const float k_FooterHeight = 55;
        const float k_MinSidePadding = 6f;
        static readonly string k_ScrollViewContentName = "unity-content-container";
        static readonly string k_ScrollViewContentAndVerticalScrollName = "unity-content-and-vertical-scroll-container";
        static readonly string k_GridViewRowClassName = "grid-view--row";
        static readonly string k_GridViewItemDummyClassName = "grid-view--item-dummy";
        static readonly string k_ScrollOffsetKey = "AM4USessionData.ScrollOffset";

        int m_MaxVisibleItems;
        readonly ScrollView m_ScrollView;
        float m_LastHeight;
        readonly Stopwatch m_Stopwatch = new();

        Vector3 m_ScrollViewOffset
        {
            get => SessionState.GetVector3(k_ScrollOffsetKey, Vector3.zero);
            set => SessionState.SetVector3(k_ScrollOffsetKey, value);
        }

        // we keep this list in order to minimize temporary gc allocs
        List<RecycledRow> m_ScrollInsertionList = new();

        Action<VisualElement, int> m_BindItemCallback;
        Func<VisualElement> m_MakeItemFunc;

        IList m_ItemsSource;
        int m_FirstVisibleIndex;

        List<RecycledRow> m_RowPool = new ();

        int m_ItemHeight = k_DefaultItemHeight;
        readonly int m_ItemWidth = k_DefaultItemWidth;
        int m_ColumnCount;
        int m_VisibleRowCount;

        internal event Action GridViewLastItemVisible;
        internal event Action BackgroundClicked;

#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<GridView> { }
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Callback for binding a data item to the visual element.
        /// </summary>
        /// <remarks>
        /// The method called by this callback receives the VisualElement to bind, and the index of the
        /// element to bind it to.
        /// </remarks>
        Action<VisualElement, int> BindItem
        {
            get => m_BindItemCallback;
            set
            {
                if (m_BindItemCallback == value)
                    return;

                m_BindItemCallback = value;
                Refresh(RefreshRowsType.ResizeGridWidth);
            }
        }

        Func<VisualElement> MakeItem
        {
            get => m_MakeItemFunc;
            set
            {
                if (m_MakeItemFunc == value)
                    return;

                m_MakeItemFunc = value;
                Refresh(RefreshRowsType.RebindGrid);
            }
        }

        internal IList ItemsSource
        {
            get => m_ItemsSource;
            set
            {
                if (m_ItemsSource == null && value == null)
                    return;

                if (m_ItemsSource != null && Utilities.CompareListsBeginnings(ItemsSource, value))
                {
                    // Value is equals to ItemsSource
                    if (m_ItemsSource.Count == value.Count)
                        return;

                    // Value is an extended List of ItemsSource
                    m_ItemsSource = value;
                    Refresh(RefreshRowsType.LoadMoreGridItems);
                    return;
                }

                // Value is a whole new List
                m_ItemsSource = value;
                Refresh(RefreshRowsType.RebindGrid);
            }
        }

        float ResolvedItemHeight
        {
            get
            {
                // todo waiting for UI Toolkit to make Panel.scaledPixelsPerPoint public
                var dpiScaling = 1f;
                return Mathf.Round(ItemHeight * dpiScaling) / dpiScaling;
            }
        }

        int ColumnCount
        {
            get => m_ColumnCount;
            set
            {
                if (m_ColumnCount != value)
                {
                    m_ColumnCount = Math.Max(value, 1);
                    Refresh(RefreshRowsType.ResizeGridWidth);
                }
            }
        }

        /// <summary>
        /// Height of the GridItems used for vertical padding in the ScrollView
        /// </summary>
        public int ItemHeight
        {
            get => m_ItemHeight;
            set
            {
                if (m_ItemHeight != value && value > 0)
                {
                    m_ItemHeight = value;
                    m_ScrollView.verticalPageSize = m_ItemHeight;
                    Refresh(RefreshRowsType.ResizeGridWidth);
                }
            }
        }

        public GridView()
        {
            m_ScrollView = new ScrollView();
            m_ScrollView.AddToClassList("grid-view-scrollbar");
            m_ScrollView.StretchToParentSize();
            m_ScrollView.verticalScroller.valueChanged += OnScroll;

            m_ScrollView.RegisterCallback<ClickEvent>(OnBackgroundClicked);

            RegisterCallback<GeometryChangedEvent>(OnSizeChanged);
            m_Stopwatch.Start();
            hierarchy.Add(m_ScrollView);

            m_ScrollView.contentContainer.focusable = true;
            m_ScrollView.contentContainer.usageHints &= ~UsageHints.GroupTransform; // Scroll views with virtualized content shouldn't have the "view transform" optimization
        }

        /// <summary>
        /// Constructs a <see cref="GridView" />, with most required properties provided.
        /// </summary>
        /// <param name="makeItem">
        /// The factory method to call to create a display item. The method should return a
        /// VisualElement that can be bound to a data item.
        /// </param>
        /// <param name="bindItem">
        /// The method to call to bind a data item to a display item. The method
        /// receives as parameters the display item to bind, and the index of the data item to bind it to.
        /// </param>
        internal GridView(Func<VisualElement> makeItem, Action<VisualElement, int> bindItem) : this()
        {
            AddToClassList(UssStyle.GridViewStyleClassName);

            MakeItem = makeItem;
            BindItem = bindItem;
        }

        /// <summary>
        /// Constructs a <see cref="GridView" />, with all required properties provided.
        /// </summary>
        /// <param name="itemsSource">The list of items to use as a data source.</param>
        /// <param name="makeItem">
        /// The factory method to call to create a display item. The method should return a
        /// VisualElement that can be bound to a data item.
        /// </param>
        /// <param name="bindItem">
        /// The method to call to bind a data item to a display item. The method
        /// receives as parameters the display item to bind, and the index of the data item to bind it to.
        /// </param>
        internal GridView(IList itemsSource, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem) : this()
        {
            AddToClassList(UssStyle.GridViewStyleClassName);

            m_ItemsSource = itemsSource;

            MakeItem = makeItem;
            BindItem = bindItem;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Refresh(RefreshRowsType.RebindGrid);
        }

        void OnScroll(float offset)
        {
            if (!HasValidDataAndBindings())
                return;
            var rowCountForSource = Mathf.CeilToInt(ItemsSource.Count / (float)ColumnCount);
            var maxOffset = rowCountForSource * ResolvedItemHeight + k_FooterHeight - m_ScrollView.contentViewport.resolvedStyle.height;
            if (offset >= maxOffset )
            {
                GridViewLastItemVisible?.Invoke();
                return;
            }
            var pixelAlignedItemHeight = ResolvedItemHeight;
            var firstVisibleIndex = Mathf.FloorToInt(offset / pixelAlignedItemHeight) * ColumnCount;

            m_ScrollView.contentContainer.style.paddingTop =
                Mathf.FloorToInt(firstVisibleIndex / (float)ColumnCount) * pixelAlignedItemHeight;
            if (m_ScrollView.verticalScroller.value == m_ScrollView.verticalScroller.highValue || !m_ScrollView.visible)
            {
                GridViewLastItemVisible?.Invoke();
            }

            if (m_RowPool.Count <= 0 ||
                m_RowPool[0].childCount !=
                ColumnCount) // If childCount is different than ColumnCount it means we are resizing the grid
            {
                return;
            }

            if (firstVisibleIndex != m_FirstVisibleIndex)
            {
                m_FirstVisibleIndex = firstVisibleIndex;
                Scrolling();
            }
        }

        void OnBackgroundClicked(ClickEvent evt)
        {
            var target = (VisualElement)evt.target;
            if (target.name == k_ScrollViewContentName
                || target.name == k_ScrollViewContentAndVerticalScrollName
                || target.ClassListContains(k_GridViewRowClassName)
                || target.ClassListContains(k_GridViewItemDummyClassName))
            {
                BackgroundClicked?.Invoke();
            }
        }

        void Scrolling()
        {
            if (!m_RowPool.Any())
                return;

            // we try to avoid rebinding a few items
            if (m_FirstVisibleIndex < m_RowPool[0].FirstIndex) //we're scrolling up
            {
                OnScrollUp();
            }
            else if (m_FirstVisibleIndex > m_RowPool[0].FirstIndex) //down
            {
                OnScrollDown();
            }
        }

        void OnScrollUp()
        {
            //How many do we have to swap back
            var initialFirstIndex = m_RowPool[0].FirstIndex;
            var count = (initialFirstIndex - m_FirstVisibleIndex) / ColumnCount;
            var inserting = m_ScrollInsertionList;

            for (var i = 0; i < count && m_RowPool.Count > 0; ++i)
            {
                var last = m_RowPool[^1];

                for (var j = 0; j < ColumnCount; j++)
                {
                    var newIndex = initialFirstIndex - (i + 1) * ColumnCount + j;

                    if (newIndex < 0)
                        continue;

                    UpdateItemAtIndexInRow(newIndex, j, last);
                }

                inserting.Add(last);
                m_RowPool.RemoveAt(m_RowPool.Count - 1); //we remove from the end

                last.SendToBack(); //We send the element to the top of the list (back in z-order)
            }

            inserting.Reverse();

            m_ScrollInsertionList = m_RowPool;
            m_RowPool = inserting;
            m_RowPool.AddRange(m_ScrollInsertionList);
            m_ScrollInsertionList.Clear();
        }

        void OnScrollDown()
        {
            var inserting = m_ScrollInsertionList;

            var checkIndex = 0;
            while (checkIndex < m_RowPool.Count && m_FirstVisibleIndex > m_RowPool[checkIndex].FirstIndex)
            {
                var first = m_RowPool[checkIndex];
                inserting.Add(first);
                first.BringToFront(); //We send the element to the bottom of the list (front in z-order)
                checkIndex++;
            }

            m_RowPool.RemoveRange(0, checkIndex); //we remove them all at once

            for (var i = 0; i < checkIndex; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    UpdateItemAtIndexInRow(
                        m_RowPool.Count * ColumnCount + i * ColumnCount + j + m_FirstVisibleIndex, j,
                        inserting[i]);
                }
            }

            m_RowPool.AddRange(inserting); // add them back to the end
            inserting.Clear();
        }

        /// <summary>
        /// Clears the GridView, recreates all visible visual elements, and rebinds all items.
        /// </summary>
        /// <remarks>
        /// Call this method whenever the data source changes.
        /// </remarks>
        internal void Refresh(RefreshRowsType refreshRowsType)
        {
            if (!HasValidDataAndBindings())
                return;

            // Check if ScrollView is already created
            m_LastHeight = m_ScrollView.layout.height;
            if (float.IsNaN(m_LastHeight))
                return;

            RefreshRows(m_LastHeight, refreshRowsType);

            var notEnoughItemToScroll = m_VisibleRowCount > 0 && m_LastHeight >= m_VisibleRowCount * ResolvedItemHeight;
            if (!notEnoughItemToScroll)
                return;

            m_ScrollView.contentContainer.style.paddingTop = 0;
        }

        internal void ResetScrollBarTop()
        {
            m_ScrollView.scrollOffset = new Vector2(0, 0);
            m_ScrollView.contentContainer.style.paddingTop = 0;
        }

        void RefreshRows(float height, RefreshRowsType refreshType)
        {
            if (refreshType == RefreshRowsType.ClearGrid || ItemsSource.Count == 0)
            {
                ClearGrid();
                return;
            }

            if (height <= 0.0f) // Might happen during UI initialization
                return;

            if (!HasValidDataAndBindings())
                return;

            var rowCountForSource = Mathf.CeilToInt(ItemsSource.Count / (float)ColumnCount);
            var contentHeight = rowCountForSource * ResolvedItemHeight + k_FooterHeight;
            m_ScrollView.contentContainer.style.height = contentHeight;

            var scrollableHeight = Mathf.Max(0, contentHeight - m_ScrollView.contentViewport.layout.height);
            m_ScrollView.verticalScroller.highValue = scrollableHeight;

            var rowCountForHeight = Mathf.FloorToInt(height / ResolvedItemHeight) + k_ExtraVisibleRows;
            var rowCount = Math.Min(rowCountForHeight, rowCountForSource);
            m_MaxVisibleItems = rowCountForHeight * ColumnCount;

            if (ItemsSource.Count <= m_MaxVisibleItems)
            {
                GridViewLastItemVisible?.Invoke();
            }

            switch (refreshType)
            {
                case RefreshRowsType.RebindGrid:
                    RebindGrid(height, rowCount);
                    break;
                case RefreshRowsType.ResizeGridWidth:
                    ResizeGridWidth(height, rowCount);
                    break;
                case RefreshRowsType.LoadMoreGridItems:
                    LoadMoreGridItems(rowCount);
                    break;
                default:
                    RebindGrid(height, rowCount);
                    break;
            }
        }

        /// <summary>
        /// Clear all rows in the ScrollView
        /// </summary>
        void ClearGrid()
        {
            foreach (var recycledRow in m_RowPool)
            {
                recycledRow.Clear();
            }

            m_RowPool.Clear();
            m_ScrollView.Clear();
            m_VisibleRowCount = 0;
            ResetScrollBarTop();
        }

        /// <summary>
        /// Shrinks the number of rows in the ScrollView if needed
        /// </summary>
        /// <param name="rowCount">Total rows count wanted</param>
        void ShrinkRows(int rowCount)
        {
            if (m_RowPool.Count > 0)
            {
                // Shrink
                var removeCount = m_VisibleRowCount - rowCount;
                for (var i = 0; i < removeCount; i++)
                {
                    var lastIndex = m_RowPool.Count - 1;
                    m_RowPool[lastIndex].Clear();
                    m_ScrollView.Remove(m_RowPool[lastIndex]);
                    m_RowPool.RemoveAt(lastIndex);
                }
            }
        }

        /// <summary>
        /// Add rows to the ScrollView if needed
        /// </summary>
        /// <param name="rowCount">Total rows count wanted</param>
        void GrowRows(int rowCount)
        {
            var addCount = rowCount - m_VisibleRowCount;
            for (var i = 0; i < addCount; i++)
            {
                var recycledRow = new RecycledRow(ResolvedItemHeight);

                for (var indexInRow = 0; indexInRow < ColumnCount; indexInRow++)
                {
                    var index = m_RowPool.Count * ColumnCount + indexInRow + m_FirstVisibleIndex;
                    var item = MakeItem != null && index < ItemsSource.Count
                        ? MakeItem.Invoke()
                        : CreateDummyItemElement();

                    recycledRow.Add(item);

                    if (index < ItemsSource.Count)
                    {
                        Setup(item, index);
                    }
                    else
                    {
                        recycledRow.Ids.Add(RecycledRow.UndefinedIndex);
                        recycledRow.Indices.Add(RecycledRow.UndefinedIndex);
                    }
                }

                m_RowPool.Add(recycledRow);
                recycledRow.style.height = ResolvedItemHeight;
                m_ScrollView.Add(recycledRow);
                recycledRow.BringToFront();
            }
        }

        void UpdateItemAtIndexInRow(int index, int indexInRow, RecycledRow recycledRow)
        {
            if (recycledRow.childCount <= indexInRow) // Create Item
            {
                if (index < ItemsSource.Count)
                {
                    var it = MakeItem != null ? MakeItem.Invoke() : CreateDummyItemElement();
                    recycledRow.Add(it);
                    Setup(it, index);
                }
                else
                {
                    recycledRow.Add(CreateDummyItemElement());
                    recycledRow.Ids.Add(RecycledRow.UndefinedIndex);
                    recycledRow.Indices.Add(RecycledRow.UndefinedIndex);
                }

                return;
            }

            var item = recycledRow.Children().ElementAt(indexInRow); // Update Item

            if (index < ItemsSource.Count)
            {
                if (recycledRow.Indices[indexInRow] == RecycledRow.UndefinedIndex)
                {
                    recycledRow.Remove(item);
                    item = MakeItem.Invoke();
                    recycledRow.Insert(indexInRow, item);
                }

                Setup(item, index);
            }
            else
            {
                if (recycledRow.Indices[indexInRow] != RecycledRow.UndefinedIndex)
                {
                    recycledRow.Remove(item);
                    item = CreateDummyItemElement();
                    recycledRow.Insert(indexInRow, item);
                }

                recycledRow.Ids.RemoveAt(indexInRow);
                recycledRow.Ids.Insert(indexInRow, RecycledRow.UndefinedIndex);
                recycledRow.Indices.RemoveAt(indexInRow);
                recycledRow.Indices.Insert(indexInRow, RecycledRow.UndefinedIndex);
            }
        }

        /// <summary>
        /// Force to Rebind all Existing Rows
        /// </summary>
        void RebindExistingRows()
        {
            foreach (var recycledRow in m_RowPool)
            {
                for (var indexInRow = 0; indexInRow < ColumnCount; indexInRow++)
                {
                    var index = m_RowPool.IndexOf(recycledRow) * ColumnCount + indexInRow + m_FirstVisibleIndex;

                    // Check if enough children in row
                    if (recycledRow.childCount <= indexInRow)
                    {
                        if (index < ItemsSource.Count)
                        {
                            var item = MakeItem.Invoke();
                            recycledRow.Add(item);
                            Setup(item, index);
                        }
                        else
                        {
                            recycledRow.Add(CreateDummyItemElement());
                            recycledRow.Ids.Add(RecycledRow.UndefinedIndex);
                            recycledRow.Indices.Add(RecycledRow.UndefinedIndex);
                        }
                    }
                    else
                    {
                        UpdateItemAtIndexInRow(index, indexInRow, recycledRow);
                    }
                }
            }
        }

        /// <summary>
        /// Rebind the Grid with new Rows
        /// Mostly used when you change project or organization
        /// Resets the Scrolling view
        /// </summary>
        /// <param name="height"></param>
        /// <param name="rowCount"></param>
        void RebindGrid(float height, int rowCount)
        {
            m_FirstVisibleIndex = 0;

            // Checking numbers of Rows
            if (m_VisibleRowCount != rowCount)
            {
                if (m_VisibleRowCount > rowCount)
                {
                    ShrinkRows(rowCount);
                    RebindExistingRows();
                }
                else
                {
                    RebindExistingRows();
                    GrowRows(rowCount);
                }
            }
            else
            {
                RebindExistingRows();
            }

            m_VisibleRowCount = rowCount;
            m_ScrollView.contentContainer.style.paddingTop = Mathf.FloorToInt(m_FirstVisibleIndex / (float)ColumnCount) * ResolvedItemHeight;
            m_LastHeight = height;
        }

        void LoadMoreGridItems(int rowCount)
        {
            if (m_RowPool.Count > 0)
            {
                var lastIndex = m_RowPool.FindLastIndex(row => row.FirstIndex != -1);
                if (lastIndex < 0)
                    return;

                for (var rowIndex = lastIndex; rowIndex < m_RowPool.Count; rowIndex++)
                {
                    for (var indexInRow = 0; indexInRow < m_RowPool[rowIndex].childCount; indexInRow++)
                    {
                        if (m_RowPool[rowIndex].Indices[indexInRow] != RecycledRow.UndefinedIndex)
                            continue;

                        var index = rowIndex * ColumnCount + indexInRow + m_FirstVisibleIndex;
                        UpdateItemAtIndexInRow(index, indexInRow, m_RowPool[rowIndex]);
                    }
                }
            }

            if(m_VisibleRowCount > rowCount)
                return;

            if (m_VisibleRowCount != rowCount)
            {
                GrowRows(rowCount);
                m_VisibleRowCount = rowCount;
            }
        }

        void ResizeGridWidth(float height, int rowCount)
        {
            if (m_VisibleRowCount <= 0)
            {
                RebindGrid(height, rowCount);
                return;
            }

            if(m_RowPool.Count == 0)
                return;

            var currentColumnCount = m_RowPool.Count > 0 ? m_RowPool[0].childCount : 0;

            if (currentColumnCount != ColumnCount)
            {
                // Get All VisualElements
                var queue = new Queue<VisualElement>(m_RowPool.SelectMany(row => row.Children()).ToList()
                    .OfType<GridItem>());

                foreach (var recycledRow in m_RowPool)
                {
                    recycledRow.Ids.Clear();
                    recycledRow.Indices.Clear();
                    recycledRow.Clear();
                    m_ScrollView.Remove(recycledRow);
                }

                var firstQueueIndex = m_FirstVisibleIndex;
                var delta = ColumnCount - currentColumnCount;
                m_FirstVisibleIndex += delta * (m_FirstVisibleIndex / currentColumnCount);

                var lastFirstIndexPossible = (Utilities.DivideRoundingUp(ItemsSource.Count, ColumnCount) - 1) * ColumnCount -
                    (rowCount - 1) * ColumnCount;
                m_FirstVisibleIndex = Math.Min(m_FirstVisibleIndex, lastFirstIndexPossible);

                while (m_FirstVisibleIndex > firstQueueIndex && queue.Count > 0)
                {
                    queue.Dequeue();
                    firstQueueIndex++;
                }

                m_RowPool.Clear();

                for (var i = 0; i < rowCount; i++)
                {
                    var recycledRow = new RecycledRow(ResolvedItemHeight);

                    for (var j = 0; j < ColumnCount; j++)
                    {
                        var index = ColumnCount * i + j + m_FirstVisibleIndex;

                        if (index < firstQueueIndex || queue.Count == 0)
                        {
                            UpdateItemAtIndexInRow(index, j, recycledRow);
                            continue;
                        }

                        var item = queue.Dequeue();
                        recycledRow.Ids.Add(index);
                        recycledRow.Indices.Add(index);
                        recycledRow.Add(item);
                        item.BringToFront();
                    }

                    recycledRow.style.height = ResolvedItemHeight;
                    m_RowPool.Add(recycledRow);
                    m_ScrollView.Add(recycledRow);
                }

                m_VisibleRowCount = rowCount;

                var pixelAlignedItemHeight = ResolvedItemHeight;
                var value = Mathf.FloorToInt(m_FirstVisibleIndex / (float)ColumnCount) * pixelAlignedItemHeight;
                m_ScrollView.verticalScroller.value = value;
                m_ScrollView.contentContainer.style.paddingTop = value;
                m_LastHeight = height;

                Scrolling();

                if (m_ScrollView.verticalScroller.value == m_ScrollView.verticalScroller.highValue ||
                    !m_ScrollView.visible)
                {
                    GridViewLastItemVisible?.Invoke();
                }
            }
        }

        void Setup(VisualElement item, int newIndex)
        {
            if (item.parent is not RecycledRow recycledRow)
            {
                throw new Exception("The item to setup can't be orphan");
            }

            var index = recycledRow.IndexOf(item);

            if (recycledRow.Indices.Count <= index)
            {
                recycledRow.Indices.Add(RecycledRow.UndefinedIndex);
                recycledRow.Ids.Add(RecycledRow.UndefinedIndex);
            }

            if (recycledRow.Indices[index] == newIndex)
                return;

            recycledRow.Indices[index] = newIndex;
            recycledRow.Ids[index] = newIndex;

            BindItem.Invoke(item, recycledRow.Indices[index]);
        }

        void OnSizeChanged(GeometryChangedEvent evt)
        {
            ColumnCount = Mathf.FloorToInt((m_ScrollView.contentViewport.layout.width - k_MinSidePadding) / m_ItemWidth);

            if (!HasValidDataAndBindings())
                return;

            var diff = m_Stopwatch.Elapsed;
            m_Stopwatch.Restart();

            if (diff.TotalSeconds < 1)
                return;

            if (Mathf.Approximately(evt.newRect.height, evt.oldRect.height) &&
                Mathf.Approximately(evt.newRect.width, evt.oldRect.width))
            {
                return;
            }

            RefreshRows(evt.newRect.height, RefreshRowsType.ResizeGridWidth);
        }

        VisualElement CreateDummyItemElement()
        {
            var item = new VisualElement();
            SetupDummyItemElement(item);
            return item;
        }

        bool HasValidDataAndBindings()
        {
            return ItemsSource != null && MakeItem != null && BindItem != null;
        }

        void SetupDummyItemElement(VisualElement item)
        {
            item.AddToClassList(UssStyle.GridViewDummyItemUssClassName);
        }

        internal void ScrollToRecycledRowOfItem(AssetIdentifier assetIdentifier)
        {
            if (m_RowPool.Count == 0)
                return;
            IEnumerable<BaseAssetData> baseAssetDataItemsSource = ItemsSource.Cast<BaseAssetData>();

            int index = baseAssetDataItemsSource
                .Select((item, idx) => new { item, idx })
                .Where(x => x.item.Identifier == assetIdentifier)
                .Select(x => x.idx)
                .DefaultIfEmpty(-1)
                .First();

            if (index < 0)
                return;

            var recycledRowElement = m_RowPool.FirstOrDefault(recycledRow => recycledRow.Indices.Contains(index));
            if (recycledRowElement == null)
            {
                //there is a scenario where the clicked item surpasses the number of recycled row UI elements
                return;
            }

            m_ScrollView.ScrollTo(recycledRowElement);
        }

        internal void SaveScrollOffset()
        {
            m_ScrollViewOffset = m_ScrollView.scrollOffset;
        }

        internal void LoadScrollOffset()
        {
            m_ScrollView.scrollOffset = m_ScrollViewOffset;
        }
    }
}
