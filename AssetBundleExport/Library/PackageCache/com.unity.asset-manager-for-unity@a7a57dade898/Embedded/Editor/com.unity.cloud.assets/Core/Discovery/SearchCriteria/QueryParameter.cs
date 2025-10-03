namespace Unity.Cloud.AssetsEmbedded
{
    sealed class QueryParameter<T>
    {
        T m_Value;

        public QueryParameter(T value = default)
        {
            m_Value = value;
        }

        /// <summary>
        /// Adds a filter to the query for the given <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="value">The value to query against. </param>
        public void WhereEquals(T value)
        {
            m_Value = value;
        }

        internal T GetValue()
        {
            return m_Value;
        }
    }
}
