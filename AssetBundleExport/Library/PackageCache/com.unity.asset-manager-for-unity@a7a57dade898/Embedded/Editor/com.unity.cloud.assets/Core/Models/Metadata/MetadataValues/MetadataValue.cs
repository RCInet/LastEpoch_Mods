using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that represents a metadata value.
    /// </summary>
    abstract class MetadataValue
    {
        /// <summary>
        /// Returns the type of the value.
        /// </summary>
        public MetadataValueType ValueType { get; private protected set; }

        private protected MetadataValue(MetadataValueType valueType)
        {
            ValueType = valueType;
        }

        /// <summary>
        /// Returns the value of the metadata.
        /// </summary>
        /// <returns>An object representing the value of the metadata. </returns>
        /// <remarks>Return values should be limited to the following types: <see cref="string"/>, <see cref="bool"/>, <see cref="DateTime"/>, <see cref="double"/> or other number types, and <c>IEnumerable</c> of string</remarks>
        internal abstract object GetValue();

        public override string ToString()
        {
            return GetValue()?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns the value as a <see cref="BooleanMetadata"/>.
        /// </summary>
        /// <returns>A <see cref="BooleanMetadata"/>. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a boolean. </exception>
        public BooleanMetadata AsBoolean()
        {
            return new BooleanMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="NumberMetadata"/>.
        /// </summary>
        /// <returns>A <see cref="NumberMetadata"/>. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a number. </exception>
        public NumberMetadata AsNumber()
        {
            return new NumberMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="DateTimeMetadata"/>.
        /// </summary>
        /// <returns>A <see cref="DateTimeMetadata"/>. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a datetime. </exception>
        public DateTimeMetadata AsTimestamp()
        {
            return new DateTimeMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="StringMetadata"/>.
        /// </summary>
        /// <returns>A <see cref="StringMetadata"/>. </returns>
        public StringMetadata AsText()
        {
            return new StringMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="SingleSelectionMetadata"/> object.
        /// </summary>
        /// <remarks>Parses the value to a simple string. Can be used when the representing the value of a <see cref="ISelectionFieldDefinition"/> when <see cref="ISelectionFieldDefinition.Multiselection"/> is False. </remarks>
        /// <returns>A <see cref="SingleSelectionMetadata"/> object containing the selected value. </returns>
        public SingleSelectionMetadata AsSingleSelection()
        {
            return new SingleSelectionMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="MultiSelectionMetadata"/> object.
        /// </summary>
        /// <remarks>Parses the value to an enumeration of strings. Can be used when the representing the value of a <see cref="ISelectionFieldDefinition"/> when <see cref="ISelectionFieldDefinition.Multiselection"/> is True. </remarks>
        /// <returns>A <see cref="MultiSelectionMetadata"/> object containing a list of selected values. </returns>
        public MultiSelectionMetadata AsMultiSelection()
        {
            return new MultiSelectionMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="UrlMetadata"/> object.
        /// </summary>
        /// <remarks>Parses the value to a url. Can be used when the representing the value of a <see cref="IFieldDefinition"/> of type <see cref="FieldDefinitionType.Url"/>. </remarks>
        /// <returns>A <see cref="UrlMetadata"/> object containg the url. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a url. </exception>
        public UrlMetadata AsUrl()
        {
            return new UrlMetadata(ValueType, GetValue());
        }

        /// <summary>
        /// Returns the value as a <see cref="UserMetadata"/>.
        /// </summary>
        /// <remarks>Parses the value to a <see cref="CommonEmbedded.UserId"/>. can be used when the representing the value of a <see cref="IFieldDefinition"/> of type <see cref="FieldDefinitionType.User"/>. </remarks>
        /// <returns>A <see cref="UserMetadata"/>. </returns>
        public UserMetadata AsUser()
        {
            return new UserMetadata(ValueType, GetValue());
        }
    }
}
