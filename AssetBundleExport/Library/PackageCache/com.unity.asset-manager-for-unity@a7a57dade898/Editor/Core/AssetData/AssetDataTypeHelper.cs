using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    enum IconSource
    {
        Default,
        Typename,
        Resource,
        TextureName
    }

    class UnityTypeDescriptor
    {
        public readonly HashSet<string> Extensions;
        public readonly AssetType Type;

        readonly IconSource m_IconSource;
        readonly string m_IconStr;

        public UnityTypeDescriptor(AssetType type, params string[] ext)
        {
            Type = type;
            Extensions = new HashSet<string>(ext);
            m_IconSource = IconSource.Default;
            m_IconStr = string.Empty;
        }

        public UnityTypeDescriptor(AssetType type, IconSource iconSource, string iconStr, params string[] ext)
        {
            Type = type;
            Extensions = new HashSet<string>(ext);
            m_IconSource = iconSource;
            m_IconStr = iconStr;
        }

        public Texture2D GetIcon()
        {
            switch (m_IconSource)
            {
                case IconSource.Typename:
                    return AssetDataTypeHelper.GetIconFromType(m_IconStr);
                case IconSource.Resource:
                    return AssetDataTypeHelper.GetIconFromResource(m_IconStr);
                case IconSource.TextureName:
                    return AssetDataTypeHelper.GetIconFromTextureName(m_IconStr);
                default:
                    return InternalEditorUtility.GetIconForFile(Extensions.FirstOrDefault());
            }
        }
    }

    static class AssetDataTypeHelper
    {
        // Order and Case is important!
        static readonly List<UnityTypeDescriptor> k_UnityTypeDescriptors = new()
        {
            new UnityTypeDescriptor(AssetType.UnityScene, ".unity"),
            new UnityTypeDescriptor(AssetType.Prefab, ".prefab"),
            new UnityTypeDescriptor(AssetType.Other, IconSource.TextureName, "SpeedTreeImporter Icon", ".st"),
            new UnityTypeDescriptor(AssetType.Model3D, ".3df", ".3dm", ".3dmf", ".3ds", ".3dv", ".3dx",
                ".blend", ".c4d", ".fbx", ".lwo", ".lws", ".ma", ".max", ".mb", ".mesh", ".obj", ".vrl", ".wrl",
                ".wrz"),
            new UnityTypeDescriptor(AssetType.Model3D, IconSource.Resource,
                "Packages/com.unity.cloud.gltfast/Editor/UI/gltf-icon-bug.png", ".glb", ".gltf"),
            new UnityTypeDescriptor(AssetType.Material, ".mat"),
            new UnityTypeDescriptor(AssetType.Animation, IconSource.TextureName, "d_AnimationClip Icon",
                ".anim"),
            new UnityTypeDescriptor(AssetType.Animation, IconSource.TextureName, "d_AnimatorController Icon",
                ".controller", ".overridecontroller"),
            new UnityTypeDescriptor(AssetType.Audio, ".aac", ".aif", ".aiff", ".au", ".flac", ".mid",
                ".midi", ".mp3", ".mpa", ".ogg", ".ra", ".ram", ".wav", ".wave", ".wma"),
            new UnityTypeDescriptor(AssetType.AudioMixer, ".mixer"),
            new UnityTypeDescriptor(AssetType.Font, ".fnt", ".fon", ".otf", ".ttf", ".ttc"),
            new UnityTypeDescriptor(AssetType.PhysicsMaterial, ".physicMaterial", ".physicsMaterial2D"),
            new UnityTypeDescriptor(AssetType.Script, ".cs"),
            new UnityTypeDescriptor(AssetType.Shader, ".shader", ".shadervariants"),
            new UnityTypeDescriptor(AssetType.ShaderGraph, IconSource.Resource,
                "Packages/com.unity.shadergraph/Editor/Resources/Icons/sg_graph_icon.png", ".shadergraph"),
            new UnityTypeDescriptor(AssetType.ShaderGraph, IconSource.Resource,
                "Packages/com.unity.shadergraph/Editor/Resources/Icons/sg_subgraph_icon.png", ".shadersubgraph"),
            new UnityTypeDescriptor(AssetType.Asset2D, ".ai", ".apng", ".avif", ".bmp", ".cdr",
                ".cur", ".dib", ".eps", ".exif", ".exr", ".gif", ".hdr", ".ico", ".icon", ".j", ".j2c", ".j2k",
                ".jas", ".jiff", ".jfif", ".jng", ".jp2", ".jpc", ".jpe", ".jpeg", ".jpf", ".jpg", ".jpw", ".jpx",
                ".jtf", ".mac", ".omf", ".pjp", ".pjpeg", ".png", ".psd", ".qif", ".qti", ".qtif", ".svg", ".tex",
                ".tfw", ".tga", ".tif", ".tiff", ".webp", ".wmf"),
            new UnityTypeDescriptor(AssetType.VisualEffect, IconSource.Typename,
                "UnityEngine.VFX.VisualEffectAsset", ".vfx", ".vfxoperator", ".vfxblock"),
            new UnityTypeDescriptor(AssetType.AssemblyDefinition, ".asmdef"),
            new UnityTypeDescriptor(AssetType.AssemblyDefinition, ".asmref"),
            new UnityTypeDescriptor(AssetType.UnityPackage, IconSource.TextureName, "d_SceneAsset Icon", ".unitypackage"),
            new UnityTypeDescriptor(AssetType.Playable, IconSource.Typename, "UnityEngine.Timeline.TimelineAsset",
                ".playable"),
            new UnityTypeDescriptor(AssetType.Asset, ".asset"),
            new UnityTypeDescriptor(AssetType.Configuration, ".config", ".cfg", ".conf", ".ini", ".toml"),
            new UnityTypeDescriptor(AssetType.Document, ".txt", ".json", ".xml", ".yaml", ".csv"),
            new UnityTypeDescriptor(AssetType.Environment, ".terrainlayer", ".lighting"),

        };

        private static readonly List<string> k_ImageFormatsSupportingPreviewGeneration = new()
        {
            ".apng", ".avif", ".bmp", ".cur", ".gif", ".ico", ".jfif", ".jpg", ".jpeg", ".pjp", ".pjpeg", ".png",
            ".svg", ".webp"
        };

        static Dictionary<string, (UnityTypeDescriptor descriptor, int priority)> s_ExtensionToUnityTypeDescriptor;

        static Texture2D DefaultIcon => InternalEditorUtility.GetIconForFile(string.Empty);

        public static string GetAssetPrimaryExtension(IEnumerable<string> extensions)
        {
            var assetExtensions = new HashSet<string>();

            foreach (var extension in extensions)
            {
                if (string.IsNullOrEmpty(extension))
                    continue;

                var ext = extension.ToLower();

                if (ext == MetafilesHelper.MetaFileExtension)
                    continue;

                if (AssetDataDependencyHelper.IsASystemFile(ext))
                    continue;

                assetExtensions.Add(ext);
            }

            if (assetExtensions.Count == 0)
            {
                return null;
            }

            foreach (var unityTypeDescriptor in k_UnityTypeDescriptors)
            {
                foreach (var extension in assetExtensions)
                {
                    if (unityTypeDescriptor.Type == GetUnityAssetType(extension))
                    {
                        return extension;
                    }
                }
            }

            return assetExtensions.FirstOrDefault();
        }

        public static IEnumerable<BaseAssetDataFile> FilterUsableFilesAsPrimaryExtensions(this IEnumerable<BaseAssetDataFile> rawList)
        {
            return rawList.Where(x =>
                x != null &&
                !string.IsNullOrEmpty(x.Extension) &&
                x.Extension != MetafilesHelper.MetaFileExtension &&
                !AssetDataDependencyHelper.IsASystemFile(x.Extension));
        }

        public static Texture2D GetIconForExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return DefaultIcon;
            }

            InitializeExtensionToUnityTypeDescriptor();

            if (s_ExtensionToUnityTypeDescriptor.TryGetValue(extension.ToLowerInvariant(), out var value))
            {
                return value.descriptor.GetIcon();
            }

            return InternalEditorUtility.GetIconForFile(extension);
        }

        public static AssetType GetUnityAssetType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return AssetType.Other;
            }

            InitializeExtensionToUnityTypeDescriptor();

            if (s_ExtensionToUnityTypeDescriptor.TryGetValue(extension.ToLowerInvariant(), out var value))
            {
                return value.descriptor.Type;
            }

            return AssetType.Other;
        }

        public static int GetPriority(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return 0;
            }

            InitializeExtensionToUnityTypeDescriptor();

            if (s_ExtensionToUnityTypeDescriptor.TryGetValue(extension.ToLowerInvariant(), out var value))
            {
                return value.priority;
            }

            return 0; // lowest priority
        }

        public static bool IsSupportingPreviewGeneration(string extension)
        {
            return k_ImageFormatsSupportingPreviewGeneration.Contains(extension.ToLower());
        }

        static void InitializeExtensionToUnityTypeDescriptor()
        {
            if (s_ExtensionToUnityTypeDescriptor != null)
                return;

            s_ExtensionToUnityTypeDescriptor = new Dictionary<string, (UnityTypeDescriptor, int)>();
            for (int i = 0; i < k_UnityTypeDescriptors.Count; ++i)
            {
                var unityTypeDescriptor = k_UnityTypeDescriptors[i];
                var priority = k_UnityTypeDescriptors.Count - i; // k_UnityTypeDescriptors has higher priority first, and we want higher number for higher priority
                foreach (var ext in unityTypeDescriptor.Extensions)
                {
                    s_ExtensionToUnityTypeDescriptor[ext.ToLowerInvariant()] = (unityTypeDescriptor, priority);
                }
            }
        }

        static MethodInfo FindTextureByType()
        {
            return Type.GetType("UnityEditor.EditorGUIUtility,UnityEditor.dll")
                ?.GetMethod("FindTexture", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static Type GetTypeInAnyAssembly(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);

                if (type == null)
                    continue;

                return type;
            }

            return null;
        }

        internal static Texture2D GetIconFromType(string typeName)
        {
            var type = GetTypeInAnyAssembly(typeName);
            return GetIconFromType(type);
        }

        static Texture2D GetIconFromType(Type type)
        {
            if (type == null)
            {
                return DefaultIcon;
            }

            return FindTextureByType().Invoke(null, new object[] { type }) as Texture2D;
        }

        internal static Texture2D GetIconFromResource(string resourceName)
        {
            var icon = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().LoadAssetAtPath(resourceName, typeof(Texture2D)) as Texture2D;
            return icon == null ? DefaultIcon : icon;
        }

        internal static Texture2D GetIconFromTextureName(string textureName)
        {
            if (!EditorGUIUtility.isProSkin && textureName.StartsWith("d_"))
            {
                textureName = textureName[2..];
            }

            var texture = EditorGUIUtility.IconContent(textureName).image as Texture2D;
            return texture != null ? texture : DefaultIcon;
        }
    }
}
