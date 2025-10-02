using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    enum FilterSelectionType
    {
        None,
        MultiSelection,
        Number,
        SingleSelection,
        Timestamp,
        Url,
        Text,
        NumberRange
    }

    readonly struct FilterSelection : IEquatable<FilterSelection>, IComparable<FilterSelection>
    {
        public string Text { get; }
        public string Tooltip { get; }

        public FilterSelection(string text, string tooltip = "")
        {
            Text = text;
            Tooltip = tooltip;
        }

        public int CompareTo(FilterSelection other) => string.Compare(Text, other.Text, StringComparison.Ordinal);

        public bool Equals(FilterSelection other)
        {
            return Text == other.Text && Tooltip == other.Tooltip;
        }

        public override bool Equals(object obj)
        {
            return obj is FilterSelection other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Tooltip);
        }

        public static bool operator ==(FilterSelection left, FilterSelection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FilterSelection left, FilterSelection right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(FilterSelection left, FilterSelection right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(FilterSelection left, FilterSelection right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(FilterSelection left, FilterSelection right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(FilterSelection left, FilterSelection right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    [Serializable]
    abstract class BaseFilter
    {
        [SerializeField]
        List<string> m_SelectedFilters;

        [SerializeReference]
        protected IPageFilterStrategy m_PageFilterStrategy;

        public abstract string DisplayName { get; }
        public abstract Task<List<FilterSelection>> GetSelections(bool includeSelectedFilters = false);
        public virtual FilterSelectionType SelectionType => FilterSelectionType.MultiSelection;

        public bool IsDirty { get; set; } = true;
        public IList<string> SelectedFilters => m_SelectedFilters;

        public virtual void Cancel() { }
        public virtual void Clear() { }

        protected BaseFilter(IPageFilterStrategy pageFilterStrategy)
        {
            m_PageFilterStrategy = pageFilterStrategy;
        }

        public virtual bool ApplyFilter(List<string> selectedFilters)
        {
            bool reload = false;

            if(selectedFilters == null && m_SelectedFilters != null)
            {
                m_PageFilterStrategy.RemoveFilter(this);
                reload = true;
            }
            else if (selectedFilters != null)
            {
                if(m_SelectedFilters != null)
                {
                    reload = !(selectedFilters.Count == m_SelectedFilters.Count && selectedFilters.All(m_SelectedFilters.Contains));
                }
                else
                {
                    reload = true;
                }
            }

            m_SelectedFilters = selectedFilters;

            return reload;
        }

        public virtual string DisplaySelectedFilters()
        {
            if(SelectedFilters == null || !SelectedFilters.Any())
            {
                return DisplayName;
            }

            if(SelectedFilters.Count == 1)
            {
                return $"{DisplayName} : {SelectedFilters[0]}";
            }

            return $"{DisplayName} : {SelectedFilters[0]} +{SelectedFilters.Count - 1}";
        }
    }
}
