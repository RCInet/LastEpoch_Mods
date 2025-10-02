using System;
using System.Text.RegularExpressions;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class for manipulating a url metadata value.
    /// </summary>
    sealed class UrlMetadata : MetadataValue
    {
        /// <summary>
        /// The url value of a metadata field.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The label value of a metadata field.
        /// </summary>
        public string Label { get; set; }

        public UrlMetadata(Uri uri = default, string label = null)
            : base(MetadataValueType.Url)
        {
            Uri = uri;
            Label = label ?? string.Empty;
        }

        internal UrlMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            if (value != null && TryParse(value.ToString(), out var uri, out var label))
            {
                Uri = uri;
                Label = label;
            }
            else
            {
                throw new FormatException($"Cannot convert {value} to url.");
            }
        }

        /// <inheritdoc />
        internal override object GetValue()
        {
            return string.IsNullOrEmpty(Label) ? Uri.ToString() : $"[{Label}]({Uri})";
        }

        internal static bool TryParse(string value, out Uri uri, out string label)
        {
            label = string.Empty;
            if (Uri.TryCreate(value, UriKind.Absolute, out uri))
            {
                return true;
            }

            const string pattern = @"\[(?<label>.*)\]\((?<url>.*)\)";
            var rgx = new Regex(pattern, RegexOptions.Singleline, TimeSpan.FromSeconds(1));
            var match = rgx.Match(value);
            if (match.Success)
            {
                label = match.Groups["label"].Value;
                return Uri.TryCreate(match.Groups["url"].Value, UriKind.Absolute, out uri);
            }

            return false;
        }
    }
}
