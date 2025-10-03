using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.identity", "1.4.0")]
#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.TestingTools")]
[assembly: InternalsVisibleTo("Unity.Cloud.TestingTools.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Runtime")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces
#endif

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.AssetManager.UI.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Upload.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.AssetManager.InternalTests.EndToEnd")]
