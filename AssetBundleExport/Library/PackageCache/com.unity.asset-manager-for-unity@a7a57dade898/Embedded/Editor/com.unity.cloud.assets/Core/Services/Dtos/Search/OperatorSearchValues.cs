using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class OperatorSearchValues : List<ISearchValue>
    {
        public enum OperatorType
        {
            Undefined = 0,
            And,
            Or,
            Not
        }

        readonly OperatorType m_Type;

        internal OperatorSearchValues(OperatorType operatorType)
        {
            m_Type = operatorType;
        }

        internal void PopulateResult(OperatorSearchValues x)
        {
            if (x.m_Type == m_Type || x.m_Type == OperatorType.Undefined)
            {
                AddRange(x);
            }
            else
            {
                var searchValue = x.GetSearchValue();
                if (searchValue != null)
                {
                    Add(searchValue);
                }
            }
        }

        internal ISearchValue GetSearchValue()
        {
            if (Count == 0)
            {
                return null;
            }

            return m_Type switch
            {
                OperatorType.And => GetSearchValue(SearchConditionData.AndType),
                OperatorType.Or => GetSearchValue(SearchConditionData.OrType),
                OperatorType.Not => Count == 1 ? new SearchNotValue {Value = this[0]} : null,
                _ => Count == 1 ? this[0] : null
            };
        }

        ISearchValue GetSearchValue(string type)
        {
            if (Count == 1)
            {
                return this[0];
            }

            var data = new SearchConditionData(type);
            foreach (var searchValue in this)
            {
                data.AddCondition(searchValue);
            }

            return data.Validate() ? data : null;
        }
    }
}
