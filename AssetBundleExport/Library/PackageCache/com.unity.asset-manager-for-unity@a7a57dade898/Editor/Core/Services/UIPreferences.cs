using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IUIPreferences : IService
    {
        int GetInt(string key, int defaultValue);
        void SetInt(string key, int value);
        bool GetBool(string key, bool defaultValue);
        void SetBool(string key, bool value);
        string GetString(string key, string defaultValue);
        void SetString(string key, string value);
        bool Contains(string key);
        void Remove(string key);
        void RemoveAll(string partialKey);
    }

    [Serializable]
    class UIPreferences : BaseService<IUIPreferences>, IUIPreferences, ISerializationCallbackReceiver
    {
        [Serializable]
        class PreferenceValue
        {
            [SerializeField]
            string m_Type;

            [SerializeField]
            bool m_BoolValue;

            [SerializeField]
            int m_IntValue;

            [SerializeField]
            string m_StringValue;

            public PreferenceValue(object value)
            {
                m_BoolValue = default;
                m_IntValue = default;
                m_StringValue = default;

                switch (value)
                {
                    case bool b:
                        m_BoolValue = b;
                        m_Type = nameof(Boolean);
                        break;
                    case int i:
                        m_IntValue = i;
                        m_Type = nameof(Int32);
                        break;
                    case string s:
                        m_StringValue = s;
                        m_Type = nameof(String);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported preference value type: {value.GetType()}");
                }
            }

            public object GetValue()
            {
                return m_Type switch
                {
                    nameof(Boolean) => m_BoolValue,
                    nameof(Int32) => m_IntValue,
                    nameof(String) => m_StringValue,
                    _ => throw new ArgumentException($"Unsupported preference value type: {m_Type}")
                };
            }
        }

        readonly Dictionary<string, object> m_Preferences = new();

        [SerializeField]
        List<string> m_Keys = new();

        [SerializeField]
        List<PreferenceValue> m_Values = new();

        public int GetInt(string key, int defaultValue)
        {
            if (m_Preferences.TryGetValue(key, out var value))
            {
                return (int) value;
            }
            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            m_Preferences[key] = value;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (m_Preferences.TryGetValue(key, out var value))
            {
                return (bool) value;
            }
            return defaultValue;
        }

        public void SetBool(string key, bool value)
        {
            m_Preferences[key] = value;
        }

        public string GetString(string key, string defaultValue)
        {
            if (m_Preferences.TryGetValue(key, out var value))
            {
                return (string) value;
            }
            return defaultValue;
        }

        public void SetString(string key, string value)
        {
            m_Preferences[key] = value;
        }

        public bool Contains(string key)
        {
            return m_Preferences.ContainsKey(key);
        }

        public void Remove(string key)
        {
            m_Preferences.Remove(key);
        }

        public void RemoveAll(string partialKey)
        {
            var keys = new List<string>(m_Preferences.Keys);
            foreach (var key in keys)
            {
                if (key.Contains(partialKey))
                {
                    m_Preferences.Remove(key);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var kvp in m_Preferences)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(new PreferenceValue(kvp.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            m_Preferences.Clear();
            for (var i = 0; i < m_Keys.Count && i < m_Values.Count; ++i)
            {
                m_Preferences[m_Keys[i]] = m_Values[i].GetValue();
            }
        }
    }
}
