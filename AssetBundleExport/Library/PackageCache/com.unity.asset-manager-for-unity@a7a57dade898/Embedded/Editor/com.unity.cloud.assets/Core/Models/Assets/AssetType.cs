using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Asset's type accepted values.
    /// </summary>
    // Dev note: [DataContract] and [EnumMember] are artifacts of the old serialization strategy.
    // The attributes are maintained for compatibility reasons and to avoid a breaking change.
    [DataContract]
enum AssetType
    {
        [EnumMember(Value = "2D Asset")]
        Asset_2D,
        [EnumMember(Value = "3D Model")]
        Model_3D,
        [EnumMember(Value = "Audio")]
        Audio,
        [EnumMember(Value = "Material")]
        Material,
        [EnumMember(Value = "Other")]
        Other,
        [EnumMember(Value = "Script")]
        Script,
        [EnumMember(Value = "Video")]
        Video,
        [EnumMember(Value = "Unity Editor")]
        Unity_Editor,
        Animation,
        Assembly_Definition,
        Asset,
        Audio_Mixer,
        Configuration,
        Document,
        Environment,
        Font,
        Physics_Material,
        Playable,
        Prefab,
        Scene,
        Shader,
        Shader_Graph,
        Unity_Package,
        Unity_Scene,
        Visual_Effect,
        Image
    }

    static class AssetTypeExtensions
    {
        /// <summary>
        /// Returns the string value of the AssetType.
        /// </summary>
        /// <param name="assetType">An <see cref="AssetType"/>. </param>
        /// <returns>The string representation of the <see cref="AssetType"/>. </returns>
        public static string GetValueAsString(this AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Asset_2D => "2D Asset",
                AssetType.Model_3D => "3D Model",
                AssetType.Audio => "Audio",
                AssetType.Material => "Material",
                AssetType.Other => "Other",
                AssetType.Script => "Script",
                AssetType.Video => "Video",
                AssetType.Unity_Editor => "Unity Editor",
                AssetType.Animation => "Animation",
                AssetType.Assembly_Definition => "Assembly Definition",
                AssetType.Asset => "Asset",
                AssetType.Audio_Mixer => "Audio Mixer",
                AssetType.Configuration => "Config",
                AssetType.Document => "Document",
                AssetType.Environment => "Environment",
                AssetType.Font => "Font",
                AssetType.Physics_Material => "Physics Material",
                AssetType.Playable => "Playable",
                AssetType.Prefab => "Prefab",
                AssetType.Scene => "Scene",
                AssetType.Shader => "Shader",
                AssetType.Shader_Graph => "Shader Graph",
                AssetType.Unity_Package => "Unity Package",
                AssetType.Unity_Scene => "Unity scene",
                AssetType.Visual_Effect => "Visual Effect",
                AssetType.Image => "Image",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Returns the AssetType from the string value.
        /// </summary>
        /// <param name="value">A string representation of the <see cref="AssetType"/>. </param>
        /// <param name="assetType">An <see cref="AssetType"/>. </param>
        /// <returns>Whether the string could be parsed into an <see cref="AssetType"/>. </returns>
        public static bool TryGetAssetTypeFromString(this string value, out AssetType assetType)
        {
            assetType = AssetType.Other;

            if (Enum.TryParse(value, out assetType)) return true;

            switch (value.Trim())
            {
                case var s when s.OrdinalEqualsOrInverse("2D", "Asset") || s.OrdinalEquals("2D"):
                    assetType = AssetType.Asset_2D;
                    break;
                case var s when s.OrdinalEqualsOrInverse("3D", "Model") || s.OrdinalEquals("3D") || s.OrdinalEquals("Model"):
                    assetType = AssetType.Model_3D;
                    break;
                case var s when s.OrdinalEquals("Audio"):
                    assetType = AssetType.Audio;
                    break;
                case var s when s.OrdinalEquals("Material"):
                    assetType = AssetType.Material;
                    break;
                case var s when s.OrdinalEquals("Other"):
                    assetType = AssetType.Other;
                    break;
                case var s when s.OrdinalEquals("Script"):
                    assetType = AssetType.Script;
                    break;
                case var s when s.OrdinalEquals("Video"):
                    assetType = AssetType.Video;
                    break;
                case var s when s.OrdinalEquals("Unity", "Editor") || s.OrdinalEquals("Unity"):
                    assetType = AssetType.Unity_Editor;
                    break;
                case var s when s.OrdinalEquals("Animation"):
                    assetType = AssetType.Animation;
                    break;
                case var s when s.OrdinalEquals("Assembly", "Definition") || s.OrdinalEquals("Assembly"):
                    assetType = AssetType.Assembly_Definition;
                    break;
                case var s when s.OrdinalEquals("Asset"):
                    assetType = AssetType.Asset;
                    break;
                case var s when s.OrdinalEquals("Audio", "Mixer"):
                    assetType = AssetType.Audio_Mixer;
                    break;
                case var s when s.OrdinalEquals("Config") || s.OrdinalEquals("Configuration"):
                    assetType = AssetType.Configuration;
                    break;
                case var s when s.OrdinalEquals("Document"):
                    assetType = AssetType.Document;
                    break;
                case var s when s.OrdinalEquals("Environment"):
                    assetType = AssetType.Environment;
                    break;
                case var s when s.OrdinalEquals("Font"):
                    assetType = AssetType.Font;
                    break;
                case var s when s.OrdinalEquals("Physics", "Material"):
                    assetType = AssetType.Physics_Material;
                    break;
                case var s when s.OrdinalEquals("Playable"):
                    assetType = AssetType.Playable;
                    break;
                case var s when s.OrdinalEquals("Prefab"):
                    assetType = AssetType.Prefab;
                    break;
                case var s when s.OrdinalEquals("Scene"):
                    assetType = AssetType.Scene;
                    break;
                case var s when s.OrdinalEquals("Shader"):
                    assetType = AssetType.Shader;
                    break;
                case var s when s.OrdinalEquals("Shader", "Graph"):
                    assetType = AssetType.Shader_Graph;
                    break;
                case var s when s.OrdinalEquals("Unity", "Package"):
                    assetType = AssetType.Unity_Package;
                    break;
                case var s when s.OrdinalEquals("Unity", "Scene"):
                    assetType = AssetType.Unity_Scene;
                    break;
                case var s when s.OrdinalEquals("Visual", "Effect"):
                    assetType = AssetType.Visual_Effect;
                    break;
                case var s when s.OrdinalEquals("Image"):
                    assetType = AssetType.Image;
                    break;
                default:
                    return false;
            }

            return true;
        }

        static bool OrdinalEquals(this string value, string other)
        {
            return value.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        static bool OrdinalEquals(this string value, string word1, string word2)
        {
            return value.OrdinalEquals($"{word1} {word2}") || value.OrdinalEquals($"{word1}{word2}") || value.OrdinalEquals($"{word1}_{word2}");
        }

        static bool OrdinalEqualsOrInverse(this string value, string word1, string word2)
        {
            var result = value.OrdinalEquals(word1, word2);
            result |= value.OrdinalEquals(word2, word1);
            return result;
        }

        /// <summary>
        /// Returns a list of all the <see cref="AssetType"/> as strings.
        /// </summary>
        /// <returns>A collection of <see cref="AssetType"/> in their string representation. </returns>
        public static List<string> AssetTypeList()
        {
            var assetTypes = new List<string>();

            foreach (var value in Enum.GetValues(typeof(AssetType)))
            {
                assetTypes.Add(((AssetType)value).GetValueAsString());
            }

            return assetTypes;
        }
    }
}
