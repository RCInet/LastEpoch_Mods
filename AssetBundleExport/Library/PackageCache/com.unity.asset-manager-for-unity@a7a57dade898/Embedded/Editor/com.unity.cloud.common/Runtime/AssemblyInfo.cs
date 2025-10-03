using System.Runtime.CompilerServices;
using Unity.Cloud.CommonEmbedded;

[assembly: ApiSourceVersion("com.unity.cloud.common", "1.2.0")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests")]
[assembly: InternalsVisibleTo("Unity.Cloud.Common.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Runtime")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces

[assembly: InternalsVisibleTo("Unity.AssetManager.Core.Editor")]

[assembly: InternalsVisibleTo("Unity.Cloud.Common.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Runtime.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Editor.Embedded")]

[assembly: InternalsVisibleTo("Unity.Cloud.AppLinking.Runtime.Embedded")]
