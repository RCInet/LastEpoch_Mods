using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IMetadata: IEquatable<IMetadata>
    {
        public string FieldKey { get; }
        public string Name { get; }
        public MetadataFieldType Type { get; }
        public object GetValue();

        public IMetadata Clone();
    }

    abstract class MetadataBase<T> : IMetadata
    {
        [SerializeField]
        string m_FieldKey;

        [SerializeField]
        string m_Name;

        [SerializeField]
        MetadataFieldType m_Type;

        [SerializeField]
        T m_Value;

        public string FieldKey => m_FieldKey;
        public string Name => m_Name;
        public MetadataFieldType Type => m_Type;

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        protected MetadataBase(MetadataFieldType type, string fieldKey, string name, T value)
        {
            m_FieldKey = fieldKey;
            m_Type = type;
            m_Name = name;
            m_Value = value;
        }

        public virtual IMetadata Clone()
        {
            return Activator.CreateInstance(GetType(), m_FieldKey, m_Name, m_Value) as IMetadata;
        }

        public object GetValue() => m_Value;

        public virtual bool Equals(IMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_FieldKey == other.FieldKey &&
                m_Name == other.Name &&
                m_Type == other.Type &&
                Equals(GetValue(), other.GetValue());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MetadataBase<T>)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FieldKey, Name, (int)Type, Value);
        }

        public static bool operator ==(MetadataBase<T> left, MetadataBase<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MetadataBase<T> left, MetadataBase<T> right)
        {
            return !Equals(left, right);
        }
    }

    [Serializable]
    class TextMetadata : MetadataBase<string>
    {
        public TextMetadata(string fieldKey, string name, string value)
            : base(MetadataFieldType.Text, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class BooleanMetadata : MetadataBase<bool>
    {
        public BooleanMetadata(string fieldKey, string name, bool value)
            : base(MetadataFieldType.Boolean, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class NumberMetadata : MetadataBase<double>
    {
        public NumberMetadata(string fieldKey, string name, double value)
            : base(MetadataFieldType.Number, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    struct UriEntry : ISerializationCallbackReceiver
    {
        [SerializeField]
        string m_SerializedUri;

        [SerializeField]
        string m_Label;

        public Uri Uri { get; set; }

        public string Label
        {
            get => m_Label;
            set => m_Label = value;
        }

        public UriEntry(Uri uri, string label)
        {
            Uri = uri;
            m_Label = label;
            m_SerializedUri = null;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedUri = Uri?.ToString();
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(m_SerializedUri))
                return;

            Uri = new Uri(m_SerializedUri);
        }
    }


    [Serializable]
    class UrlMetadata : MetadataBase<UriEntry>
    {
        public UrlMetadata(string fieldKey, string name, UriEntry value)
            : base(MetadataFieldType.Url, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    struct DateTimeEntry : ISerializationCallbackReceiver
    {
        [SerializeField]
        long m_SerializedDataTime;

        [SerializeField]
        DateTimeKind m_SerializedDataTimeKind;

        public DateTime DateTime { get; set; }

        public DateTimeEntry(DateTime dateTime)
        {
            DateTime = dateTime;
            m_SerializedDataTime = 0;
            m_SerializedDataTimeKind = DateTimeKind.Unspecified;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedDataTime = DateTime.Ticks;
            m_SerializedDataTimeKind = DateTime.Kind;
        }

        public void OnAfterDeserialize()
        {
            DateTime = new DateTime(m_SerializedDataTime, m_SerializedDataTimeKind);
        }
    }

    [Serializable]
    class TimestampMetadata : MetadataBase<DateTimeEntry>
    {
        public TimestampMetadata(string fieldKey, string name, DateTimeEntry value)
            : base(MetadataFieldType.Timestamp, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class UserMetadata : MetadataBase<string>
    {
        public UserMetadata(string fieldKey, string name, string value)
            : base(MetadataFieldType.User, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class SingleSelectionMetadata : MetadataBase<string>
    {
        public SingleSelectionMetadata(string fieldKey, string name, string value)
            : base(MetadataFieldType.SingleSelection, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class MultiSelectionMetadata : MetadataBase<List<string>>
    {
        public MultiSelectionMetadata(string fieldKey, string name, List<string> value)
            : base(MetadataFieldType.MultiSelection, fieldKey, name, value)
        {
        }

        public override IMetadata Clone()
        {
            return new MultiSelectionMetadata(FieldKey, Name, new(Value));
        }

        public override bool Equals(IMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is not MultiSelectionMetadata otherMultiSelection) return false;

            return FieldKey == other.FieldKey &&
                Name == other.Name &&
                Type == other.Type &&
                new HashSet<string>(Value).SetEquals(new HashSet<string>(otherMultiSelection.Value));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IMetadata)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FieldKey, Name, (int)Type, GetValue());
        }

        public static bool operator ==(MultiSelectionMetadata left, MultiSelectionMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MultiSelectionMetadata left, MultiSelectionMetadata right)
        {
            return !Equals(left, right);
        }
    }
}
