using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class LabelBaseData : ILabelBaseData
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public Color? DisplayColor { get; set; }

        [DataMember(Name = "colour")]
        internal string ColorHexString
        {
            get => ColorToHex();
            set => HexToColor(value);
        }

        string ColorToHex()
        {
            return DisplayColor.HasValue ? $"#{DisplayColor.Value.ToArgb().ToString("X")[2..]}" : null;
        }

        void HexToColor(string hexString)
        {
            if (string.IsNullOrEmpty(hexString)) return;

            hexString = hexString.TrimStart('#');
            hexString = $"FF{hexString}";
            if (int.TryParse(hexString, NumberStyles.HexNumber, null, out var color))
            {
                DisplayColor = Color.FromArgb(color);
            }
        }
    }
}
