using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.common", "1.2.0")]
#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests.ValidApiSource")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests.InvalidApiSource")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests.NoApiSource")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Runtime")]
[assembly: InternalsVisibleTo("Unity.Cloud.DataStreaming.Tests.Runtime")]
[assembly: InternalsVisibleTo("Unity.Cloud.TestingTools")]
[assembly: InternalsVisibleTo("Unity.Cloud.TestingTools.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Runtime")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Editor.Networking")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Tests.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

#endif

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Upload.Editor")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Documentation")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.AssetManager.UI.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Upload.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Common.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Common.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.AssetManager.InternalTests.EndToEnd")]
