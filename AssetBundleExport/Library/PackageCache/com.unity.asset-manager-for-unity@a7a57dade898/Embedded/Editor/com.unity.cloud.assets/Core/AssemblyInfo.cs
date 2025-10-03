using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.assets", "1.8.1")]

#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Tests.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Documentation")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.AssetManager.UI.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Upload.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.AssetManager.InternalTests.EndToEnd")]
