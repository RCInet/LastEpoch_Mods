using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.identity.editor", "1.4.0")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Editor.Embedded")]
