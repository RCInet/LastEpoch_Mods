using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.identity", "1.4.0")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.Cloud.Common.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor.Embedded")]
