using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.app-linking", "1.2.0")]
#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Tests")]
[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

#endif

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor.Tests")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Runtime.Embedded")]
