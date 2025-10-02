using System.Drawing;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ILabelBaseData
    {
        [DataMember(Name = "name")]
        string Name { get; }

        [DataMember(Name = "description")]
        string Description { get; }

        Color? DisplayColor { get; }
    }
}
