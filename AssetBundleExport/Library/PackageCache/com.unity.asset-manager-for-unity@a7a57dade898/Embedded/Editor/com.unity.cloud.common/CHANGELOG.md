# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2025-06-26

### Added
- `WebAppNames` class.

### Changed
- `WebAppUrlComposer` and `ServiceHostResolverFactory` classes are no longer experimental.
- Experimental `WebAppUrlComposer` static properties `Asset-Manager` and `Documentation` moved to a new `WebAppNames` class.

### Fixed
- Fixed race condition in `SingleReaderSingleWriterNativeStream`.
- Fixed exception thrown when a request is canceled.
- Fixed `ObjectDisposedException` thrown in some cases when accessing the `HttpResponseMessage` object.
- Fix race condition where sometimes the UnityHttpClient would have cached a uninitialized value that resulted in an ArgumentNullException thrown at runtime.
- Fixed http request task not completing in some cases when the request is canceled.

### Removed
- `UnityCloudPlayerSettings.SetAppNamespace` method and `UnityCloudPlayerSettings.OnAppNamespaceChanged` event.
- `ServiceHostResolverFactory.CreateForUnityServicesGateway` method.

## [1.2.0-exp.4] - 2025-06-04

### Added
- `UnityCloudPlayerSettings.SetAppNamespace` method to allow the modification of the App namespace from Editor scripts.
- `HttpRequestMessageExtensions` static class to safely read `HttpRequestMessage.Content` as string or byte array on single threaded platform.

### Changed
- Made ServiceError.ToString() more verbose
- Reworked `UnityHttpClient` to reduce memory consumption when using the `ResponseHeadersRead` option.
- Documentation to reference WebGL support

### Fixed
- Throwing Http exceptions on WebGL.

## [1.2.0-exp.3] - 2025-05-12

### Added
- `WebAppUrlComposer` class and `IWebAppUrlComposer` interface to compose URLs for supported web app resources.

### Changed
- Lowered the default exponential retry policy timespan when running on the Web Platform.
- ServiceHttpClient retries only for error codes 503/504, instead of >= 500
- Rename FQDN path prefix env var name

### Fixed
- iOS and OSX prebuild processing supports multiple registration of custom uri scheme in the info.plist manifest.

## [1.2.0-exp.2] - 2025-01-21

### Changed
- Updated how service errors are logged when http response failed to produce a valid json.

## [1.2.0-exp.1] - 2024-12-12

### Added
- Added `Content-Length` in the `HttpResponseMessage` returned by `UnityHttpClient`.
- `ServiceHostResolverFactory.CreateForUnityServicesGateway` and `ServiceHostResolverFactory.CreateForFullyQualifiedDomainName` methods.
- new `GetResolvedHost` extension method for `IServiceHostResolver` implementations.
- Added `Content-Encoding` in the `HttpResponseMessage` returned by `UnityHttpClient`.

### Deprecated
- `UnityRuntimeServiceHostResolverFactory` class.
- `IServiceHostResolver.GetResolvedDomainProvider` and `IServiceHostResolver.GetResolvedEnvironment` methods. 

## [1.1.5] - 2024-09-23

### Fixed
- Fixed internal disposal of http response content stream.
- Ensure that the WebSocket implementation for WebGL is not visible in the Unity Editor.

## [1.1.4] - 2024-08-08

### Changed
- Replaced the `WindowsBuildPostPRocessTokenResolver.cs` that was creating an .exe from base64 encoded string in favor of copying the `/AppLinking/Tools/CustomUriSchemeResolver.exe` file.
- Export `/AppLinking/Tools/CustomUriSchemeResolver.pdb` file along `/AppLinking/Tools/CustomUriSchemeResolver.exe` file when the Unity Project is built in Development mode.

## [1.1.3] - 2024-05-31

### Added
- X-Client-Id header now included by default in all Http requests to Unity Services.

### Fixed
- LegacyRequestHandler skip header update if no header response (happens when pointing to a local file).

## [1.1.2] - 2024-05-17

### Added
- Added internal ability to profile the UnityHttpClient

## [1.1.1] - 2024-05-02

### Added
- Added Apple Privacy Manifest documentation.

## [1.1.0] - 2024-04-05

### Added
- Added Apple Privacy Manifest file to `/Plugins` directory.
- Added 'ToJson' and 'FromJson' methods to `ProjectDescriptor`, `AssetDescriptor`, `DatasetDescriptor`, and `FileDescriptor` to allow for serialization and deserialization of these structs.

### Changed
- Modified the `LogLevel` for several log messages to reduce the default amount of logs in the console.

### Fixed
- Fixed `NativeWebSocketClient` tests which were not correctly handling expected errors and logs.

### Removed
- Removed deprecated headers.

## [1.0.0] - 2024-02-26

### Added
- Added `GroupId` struct.
- Added `Role` and `Permission` struct.
- Added `UserId` struct from Unity.Cloud.Identity.

### Changed
- [Breaking] Renamed `OrganizationGenesisId` property to `OrganizationId` in `ProjectDescriptor`, `AssetDescriptor` , `DatasetDescriptor`  and `FileDescriptor` classes.
- Replaced UnityConfigureAwait(false) calls with await operators to improve performance by avoiding unnecessary context switching.
- Unity Cloud project settings moved under the `/Services/Unity Cloud` tab in the Unity Editor player settings.
- The `Message` field in `ServiceException` is now initialized with more detailed information based on the `ServiceError` which triggered the exception.

### Removed
- [Breaking] Removed `AssetVersion(int)` constructor from `AssetVersion` struct.

## [1.0.0-pre.6] - 2024-02-13

### Fixed
- Fixed samples relying on StreamingAssets not working when many samples are imported together.

## [1.0.0-pre.5] - 2024-01-15

### Added
- Unity.Cloud.AppLinking namespace.
- Added `HostUrl` property in `IUrlProcessor` interface,
- The `Details` property has been added to `ServiceException`.

### Changed
- [Breaking] Moved `IUrlRedirectAwaiter`, `IUrlRedirectionInterceptor`, `IActivatePlatformSupport`, `IUrlProcessor`, `UrlRedirectUtils`, `UrlRedirectStatus`, `UrlRedirectResult`, `UrlProcessor`, `ProcessId`, `ChannelUrlRedirectionInterceptor`,  `UrlRedirectionInterceptor`, `UnityRuntimeUrlProcessor`, `UnityCloudPlayerSettings`, `ActivateAppFromUrl` from Unity.Cloud.Common, Unity.Cloud.Interop and Unity.Cloud.Identity to Unity.Cloud.AppLinking.
- [Breaking] `ServiceError` has been made internal.

### Removed
- Removed documentation related to App Registration.
- [Breaking] Removed `UrlProcessor` core implementation.

## [1.0.0-pre.4] - 2023-12-20

### Added
- Added MIME type support to `UnityHttpClient`.

### Changed
- [Breaking] `WebSocketClientFactory` now inherits from the `IWebSocketClientFactory` interface.
- [Breaking] Reworked `ServiceException` and all related classes to match the [Unity Services codes](https://services.docs.unity.com/docs/errors/index.html). 

### Removed
- [Breaking] Migrated interoperability classes from Common to Identity.
- Removed documentation related to App Registration

## [1.0.0-pre.3] - 2023-12-07

### Added
- Added unit tests for header support.
- Added Critical and None log levels.
- Added numerous Logger extension methods.

### Changed
- [Breaking] Moved code causing circular dependency from Common/Editor/Settings and Common/Editor/BuildTools to Identity/Editor.
- [Breaking] UnityCloudAppRegistration is now in Unity.Cloud.Identity.Editor.
- [Breaking] Renamed `LogInfo` Logger extension to `LogInformation`.
- [Breaking] Moved `UnityCloudPlayerSettings` class to Identity.

### Fixed
- Fixed typos in manual documentation.
- Fixing missing `System` namespace in `WebSocketClientFactory` to fix compilation error.

### Removed
- [Breaking] Removed `IAppNameProvider` and `IAppDisplayNameProvider` interfaces.
- [Breaking] Removed public references to Newtonsoft JsonConverters
- [Breaking] Removed Log overloads without MessageArgs.

## [1.0.0-pre.2] - 2023-11-23

### Fixed
- Windows specific: custom uri schema resolver now supports Unicode character in the application absolute path.
- WebGL IFrame issue in CommonBrowserInterop.jslib.

### Removed
- Removed unused domains from `AppLinksHelper.cs`

## [1.0.0-pre.1] - 2023-11-07

### Added
- Added a first pass of manual documentation.

### Changed
- Updated LICENSE.md file.
- Improved information in README
- Improvements to scripting API doc
- Updated manual documentation

### Fixed
- Validate files are copied only once when importing samples.

### Removed
- Removed inapplicable notices in documentation.
- [Breaking] Made `IKeyValueStoreExtensions` internal as its only method was also internal.

## [1.0.0-exp.1] - 2023-10-26

### Added
- Made `ServiceHttpClientModifier` public.
- Added new methods to `HttpResponseMessageExtensions`.
- Importing assets with a StreamingAssets directory will copy those files, on approval, in the project StreamingAssets directory.
- Added new properties to `ServiceException` in order to access all necessary information on errors.

### Changed
- Upgrade to newtonsoft-json 3.2.1.
- [Breaking] `UrlRedirectionInterceptor` now expects to find the custom uri resolver executable under the `Unity_Cloud_Interop` folder.
- Improved the error-messaging whe a `IRetryPolicy` has expired.
- Change minimal Unity version to 2022.3

### Fixed
- Fixing regex parameters.
- Removing redundant `AssetDatabase.Refresh()` call in sample import.

### Removed
- [Breaking] Removing obsolete `UriSchemeRedirection` class.
- [Breaking] Removing obsolete `GetDisplayName()` method from `UnityCloudPlayerSettings`.

## [0.16.0] - 2023-10-18

### Added
- Added `IAppNamespaceProvider` interface and a `DefaultAppNamespaceProvider` implementation.
- [Breaking] Added ProcessId and AppId classes to replace string IDs.
- [Breaking] Added support for the `services.api.unity.com` domain through the `UnityServices` service provider. This is the default service provider.
- [Breaking] Removed unsupported `Azure` and `GCP` service providers.
- Added `IServiceDomainResolver` to allow the possibility to instantiate a `ServiceHostResolver` implementation with specific domain mapping.
- Added public constructor for the `UnityRuntimeServiceHostResolverFactory` with override.
- Added support for new IDs: `ProjectId`, `AssetId`, and `DatasetId`.
- Changed HeaderUtils regex to also match UCF endpoints when checking if IsUnityApi
- Exposed the App's organization ID in `UnityCloudPlayerSettings`.
- [Breaking] `ServiceHttpClient` constructor will throw an `ArgumentNullException` for null parameters.
- Added support for `X-Unity-Cloud-*` headers.
- Added `FileDescriptor`
- Added properties to all descriptors to easily access nested IDs.

### Changed
- `UnityCloudPlayerSettings` implements `IAppNamespaceProvider` and provides a default namespace value of "com.unity.cloud".
- [Breaking] default app namespace is now com.unity.cloud.
- [Breaking] Changed public field name `UnityCloudAppRegistration.m_AppInfoProvider` to `UnityCloudAppRegistration.AppInfoProvider`.
- Changed HeaderUtils regex to also match UCF endpoints when checking if IsUnityApi
- [Breaking] Remove the AssetVersionId and AssetVersionDescriptor structs and replace by AssetVersion in the AssetDescriptor.
- [Breaking] AppInfoProvider now expects the organizationId as a parameter.
- ServiceHttpClient: ClientTrace becomes public.
- ApiSourceVersion: GetApiSourceVersionForAssembly becomes public.
- [Breaking] Refactored IHttpClient so Http Status Codes are no longer triggering exceptions.
- Removed flooding log when receiving message in NativeWebSocketClient.

### Fixed
- Fixed an issue in `UnityCloudPlayerSettings` editor window where changes were not saved between sessions.
- Fixed an issue in `UnityCloudPlayerSettings` editor window where edits and deletions of the current application were not updated in the UI and the asset.
- Fixed Service Error deserialization.

### Removed
- [Breaking] Removed `HttpClientHeaderModifier` and `IHttpClient.WithApiSourceHeaders` extension.
- [Breaking] Removed deprecated `ServiceHostConfiguration` class. Use `IServiceHostResolver` instead.

## [0.15.0] - 2023-09-15

### Changed
- [BREAKING] The `ServiceHttpClient` now expects an `IServiceAuthorizer` instead of the deleted `IAccessTokenProvider`.
- [BREAKING] The `AddAuthroization` method in `ServiceHeaderUtils` now expects an additional authorization scheme parameters.
- The "Basic" and "Bearer" authorization constants are now public in `ServiceDomainUtils`.

### Fixed
- Fixed upload progress in DotNetHttpClient.

## [0.14.4] - 2023-08-31

### Added
- Added official support for the latest LTS Editor 2022.3 while maintaining support for 2021.3.

### Changed
- `UrlRedirectionInterceptor` internal `AsyncUrlRedirectAwaiter` default timeout delay set to 10 minutes instead of 1 minute.

### Fixed
- Added missing compile flags in `ServiceHeaderUtils`.

### Deprecated
- `SceneId`, `SceneVersionId`, `WorkspaceId`, and `IScene` have been marked deprecated. They are to be replaced with IDs related to the Asset Manager SDK.

## [0.14.3] - 2023-08-17

### Added
- Added a log for JSON serialization errors.

### Changed
- Simplified ServiceHost tests by removing a needless test defaults class and directly referencing the correct values.
- Changed ServiceHttpClient's default retry policy (NoRetryPolicy) to ExponentialBackoffRetryPolicy.
- [BREAKING] `ServiceHeaderUtils.AddAuthorization` method no longer add HTTP authorization headers on WebGL.

### Fixed
- Added missing dependency to `com.unity.modules.imgui` to package manifest.
- Added a timeout to the Uri regex filter for header modification.

## [0.14.2] - 2023-08-03

### Changed
- Explicitly set disposeDownload/UploadHandlerOnDispose to true.

### Fixed
- The `HttpClientHeaderModifier` and derived classes now accept a uri filter to specify which requests to add custom headers to.
- The `WithApiSourceHeaders()` extension methods for adding analytics headers to `IHttpClient` and `IServiceHttpClient` automatically add a filter to only add the headers to Unity API calls.
- Fixed memory leak that happened whenever we were trying to send a delete request.

## [0.14.1] - 2023-07-20

### Added
- `IUrlProcessor` interface along `CoreUrlProcessor`, `UrlProcessor` and `UrlRedirectUtils` classes.
- `CoreTimeAwaiter` class.
- `IAppDisplayNameProvider` interface.
- Added `ForceCompleteContent` method to use in the event of an error that prevents the download handler from calling complete content.

### Changed
- `ITimeAwaiter`, `IUrlRedirectAwaiter`, `AsyncUrlRedirectAwaiter` are now public.
- `UnityCloudPlayerSettings` now implements `IAppDisplayNameProvider` interface.
- Changed `UnityCloudPlayerSettings` editor window (Unity Cloud/ App Information) to let user select/edit/delete existing applications and register new applications.
- `BuildUtils` now also checks for empty/null app name and app ID fields in `UnityCloudPlayerSettings`.

### Fixed
- Fixed the error thrown from including `progress` parameter on `UnityHttpClient` requests
- Added missing implementations for `TwoWayMemoryStream`

## [0.14.0] - 2023-07-06

### Added
- Added download/upload support for IHttpClient.SendAsync
- Added custom DownloadHandler to UnityHttpClient

### Changed
- [Breaking] `IServiceHostResolver` added to replace `ServiceHostConfiguration`.
- [Deprecated] `ServiceHostConfiguration` is now deprecated in facor of `IServiceHostResolver`.
- `ServiceHostConfiguration` implements `IServiceHostResolver` to maintain backwars compatibility.
- Uses of `ServiceHostConfiguration` have been replaced for `IServiceHostResolver`.
- [Deprecated] `ServiceEnvironment.Url` and `ServiceEnvironment.Local` are deprecated.
- [BREAKING] Exposed IProgress in IHttpClient.SendAsync
- [BREAKING] Exposed HttpCompletionOption in IHttpClient.SendAsync
- Optimized UploadHandler selection for UnityHttpClient
- Adapted the way we serialize HTTP content for WebGL builds.

### Removed
- Removed needless log from sample import.
- [BREAKING] Removed DownloadFileAsync from IHttpClient
- Removed deprecated code related to HTTP response message with no content

## [0.13.1] - 2023-06-22

### Changed
- Disabled manual redirection for WebGL to fix GCP support.
- Support for internal debug options.

### Fixed
- When a request URI is pointing to endpoints external to unity.com, custom headers are no longer added.

## [0.13.0] - 2023-05-25

### Added
- Added `InstanceId` struct.
- Support for `Azure` service provider and `transformation.unity.com` domain.
- [Breaking] `Azure` is now the default service provider.

### Changed
- Explicit http request redirection handling in `LegacyRequestHandler`.
- [Breaking] Renamed `ServiceRegionUtils.Provider` to `ServiceDomainProvider`.
- [Breaking] Removed `Default` values from `ServiceEnvironment` and `ServiceDomainProvider`.
- `ServiceHostConfiguration` now supports a `ServiceDomainProvider` override.
- [Breaking] Parameters renamed in `ServiceHostConfiguration.GetServiceAddress()` overloads.
- Renamed all instances of `dt.unity.com` in test cases to `mock.unity.com`.
- [Breaking] `ServiceRegionUtils` renamed to `ServiceDomainUtils` and made internal.
- Split `ServiceHostConfiguration.cs` into separate files by class and struct.
- Modified import order for samples and dependencies to avoid missing asset errors. 

### Removed
- [Breaking] Removed unsupported `GCPUK` and `Tencent` providers

## [0.12.2] - 2023-05-11

### Changed
- Upgrade to Moq 2.0.0-pre.2

### Fixed
- NativeWebSocketClient: do not dispose client on disconnect.
- Explicitly set the serialization options.

## [0.12.1] - 2023-04-27

### Added
- Added support for the PATCH http method in LegacyRequestHandler.

### Changed
- Fix exception when stacktrace is null in `UnityLogOutput`.

## [0.12.0] - 2023-04-13

### Added
- Added documentation for thrown exceptions in the `Networking/Http` components.
- Added a `CreateHttpRequestMessage` helper method to `HttpClientExtensions`.

### Changed
- [Breaking] New exceptions can be thrown in the Http clients, documented for each method.

### Removed
- [Breaking] Removed `RetryExpiredException` in favor of `TimeoutException` usage.
- [Breaking] Removed `CreateUri()` from `ServiceHttpClientExtensions`.

## [0.11.0] - 2032-03-30

### Changed
- [Breaking] `UrlRedirectionInterceptor` runtime class ctor changed from `public` to `internal`
- [Breaking] Change identifier types from Guid to SceneId, WorkspaceId, DatasetId, OrganizationId and VersionId.
- [Breaking] Refactored both `DotNetHttpClient` and `UnityHttpClient` to better handle exceptions.
- Added `HttpClientHeaderModifier` and `SericeHttpClientHeaderModifier` implementations to automatically append headers to all requests
- Added an `ApiSourceVersion` class and associated attribute and extension methods to append Api Source information as headers
- Added a method to `IServiceMessagingClient` to add  `ApiSourceVersion` which will be added as headers in the `ServiceMessagingClient` implementation.
- Exposes the Api Source header value in `ServiceHeaderUtils`

### Fixed
- Disable code using Moq when the package is not present.

## [0.10.0] - 2023-03-16

### Changed
- [Breaking] `ServiceExceptions` objects are now built with Correct Default Codes.
- [Breaking] All mentions of "Digital Twins" and "DT" renamed to their "Unity Cloud" equivalent, or removed altogether
- [Breaking] The `DT_CLOUD` environment variable is now `UNITY_CLOUD_SERVICES_ENV`
- The `UC_TRACE` header key is not `UNITY_CLOUD_TRACE`
- `ActivateAppFromUrl` modifier changed from `internal` to `public`
- [Breaking] `Digital Twins/Resources/DigitalTwinsPlayerSettings.asset` renamed to `Unity Cloud/Resources/UnityCloudPlayerSettings.asset`
- Existing `DigitalTwinsPlayerSettings` within your projects should be change to the new naming and file path.
- [Breaking] `CloudConfiguration` renamed to `ServiceHostConfiguration`
- [Breaking] `CloudEnvironment` renamed to `ServiceEnvironment`
- [Breaking] `IMessagingClient` renamed to `IServiceMessagingClient`
- [Breaking] `RegionUtils` renamed to `ServiceRegionUtils`
- [Breaking] `Protocol` renamed to `ServiceProtocol`
- [Breaking] Moved all Storage related interfaces and structures to Storage package
- [Breaking] `Clipboard` has been replaced by platform-specific `UnityClipboard` and `BrowserClipboard`.

### Removed
- [Breaking] Removed Url field from IScene and IWorkspace interfaces.

## [0.9.2] - 2023-03-02

### Added
- Added tests for Clipboard
- `RefreshPropertiesAsync`, `DeleteMetadataAsync`,`AddOrUpdateMetadataAsync`,`Metadata` and `MetadataChanged` to `IWorkspace`
- async methods to create, delete, list, and update versions to `IDataset`.
- `IDatasetVersion`, `IDatasetVersionCreation` and `IDatasetVersionUpdate` interfaces.
- Added unit tests for new `IHttpClient` exceptions

### Changed
- `AsyncUrlRedirectAwaiter` now uses the `ITimeAwaiter` for its refresh delays.
- `IWorkspace` inherits `INotifyPropertyChanged`
- [Obsolete] IClipboard now has platform-specific implementations and `Clipboard` is obsolete.
- [Obsolete] `IWorkspace.Url` property is marked as Obsolete (we will stop exposing it in the future)
- [Obsolete] `IScene.Url` property is marked as Obsolete (we will stop exposing it in the future)
- [Breaking] `IUrlRedirectionInterceptor` and related classes now throw `TimeoutException`, and no longer return `Failed` or `Timeout`  redirection statuses

## [0.9.1] - 2023-02-24

### Added
- `RefreshPropertiesAsync`, `DeleteMetadataAsync`,`AddOrUpdateMetadataAsync`,`Metadata` and `MetadataChanged` to `IWorkspace`
- async methods to create, delete, list, and update datasets to `IWorkspace`.
- `IDataset` interface

### Changed
- `IWorkspace` inherits `INotifyPropertyChanged`
- `IWorkspace.Url` property is marked as Obsolete.
- `IScene.Url` property is marked as Obsolete.

### Fixed
- Fixed ServiceHttpClient turning all http error codes into RetryExpiredException.
- Fixed NativeWebSocketClient catched exception type.

## [0.9.0] - 2023-02-16

### Added
- GetAppsInfoAsync method in `IAppInfoProvider` and `AppInfoProvider`.
- `IClipboard` interface and a `Clipboard` implementation.
- ValidateFilenameExistsAsync method in `IKeyValueStore`, `FileKeyValueStore` and `BrowserKeyValueStore`.
- Websocket support for sending binary frames

### Changed
- Clarifying the contract of IRetryPolicy.
- [Breaking] OperationRetryQueued and RetryQueuedParams replaced with IProgress in ExecuteAsync and RetryQueuedProgress.
- [Breaking] Added RetriedOperation and ShouldRetryChecker delegates. The latter replaces ShouldRetryParams struct.
- [Breaking] ExecuteAsync method's contract has been updated to accept new delegates and IProgress ; it also throws an exhaustive list of exceptions listed in its xml documentation.
- [Breaking] ExecuteAsyncWithExceptionValidation now requires a ShouldRetryExceptionChecker method
- [Breaking] Updated ExponentialBackoffRetryPolicy and NoRetryPolicy to follow the new contract
- Updated ServiceHttpClient and MessagingClient to use the new version of RetryPolicy.
- Added KeepAlive to websocket client, which requires a the server to support Ping/Pong messages (RFC6455 - 5.5.2/5.5.3)

### Fixed
- Fixed `UnityLogOutput.GenerateMessageFromException` to correctly handle exceptions inside a `System.Task`.
- Fix error string in CloudConfiguration.

## [0.8.1] - 2023-02-02

### Added
- CopyToClipboard method in `CommonBrowserInterop` to fix WebGL runtime limitation.

### Changed
- [BREAKING] Added exceptions to the Interoperability component and updated tests.

## [0.8.0] - 2023-01-19

### Added
- `ActivateAppFromUrl` and `ActivateAppFromUrlEditor` to complement existing `UrlRedirectionInterceptor` runtime class.
- `IStringObfuscator` interface and `AesStringObfuscator` class in core.
- Increased unit test coverage for Networking classes.
- Scripting API documentation on several components.

### Changed
- [Breaking] `FileKeyValueStore` constructor has new optional `IStringObfuscator` parameter.

### Fixed
- Resolved various code analysis smells, bugs, and warnings
- Removed duplication in RetryPolicy tests

## [0.7.1] - 2022-12-21

### Added
- Settings: Unit test for `AppInfoProvider`
- HttpClientExtensions: DeleteJsonAsync, GetJsonAsync, PostJsonAsync and PutJsonAsync to execute asynchronious requests with JSON serialization and deserialization.
- Exposed `HttpStatusCode` in `ServiceException`
- Override for `GetObjectData()` in `ServiceException` to serialize `ServiceError` variable.
- A default message for `LicenseUnavailableException`.

### Changed
- Default string content for POST and PUT for UnityHttpClient in `LegacyRequestHandler.cs`.
- Updated the implementation of 'ISerializable' for ServiceExceptions to conform to the recommended pattern.

### Fixed
- Sending content for DELETE for UnityHttpClient in `LegacyRequestHandler.cs`.
- Receiving content for DELETE for UnityHttpClient in `LegacyRequestHandler.cs`.

## [0.7.0] - 2022-12-08

### Added
- Retry policy: IRetryPolicy, ExponentialBackoffRetryPolicy, NoRetryPolicy, TimeSeriesBuilder, RetryExpiredException, ServiceHttpClientTests
- Integrate a retry policy in ServiceHttpClient. SendAsync with ServiceHttpClientOptions argument can use retry policy. SendAsync without ServiceHttpClientOptions argument do not use retry policy.
- New retry policy parameter in ServiceHttpClientOptions constructor.
- Integrate a retry policy in MessagingClient. By default, use a "exponential backoff with jitter" retry. New MessagingClientOptions can be passed to ConnectAsync.
- WebSocketClientFactory exposing IWebSocketClient creation based on platform.
- New GetLogger that accepts the type as a string.
- Protected Websocket connect in MessagingClient

### Changed
- WebglWebSocketAdapter: Connect method throw exception when connection fail.
- [Breaking] IWebSocketClient is now public and moved from runtime to core.
- [Breaking] MessagingClient moved from runtime to core and constructor requires an IWebSocketClient.
- Utilities moved from runtime to core.
- NativeWebSocketClient send connection state changed through thread invoker.
- Fixed AddHeadersAsQuery method in HeaderUtils.
- Replaced obsolete string conversion call in WebSocketAdapter.jslib.

### Fixed
- Duplicate command line arguments crash
- Fix non-string content for PUT and POST for UnityHttpClient.

## [0.6.0] - 2022-11-24

### Added
- Capability to send a message collection in IMessagingClient/MessagingClient.
- LaunchArgumentsParser.
- [Breaking] GetSceneAsync method in IWorkspace interface.
- Button to link to online documentation from Player Settings.
- Internal WebGL support: DTTask, TaskExtensions

### Changed
- Fixed and activated MessagingClientTests.
- [Breaking] Renamed the sample configuration from `samples.json` to a hidden `.dt-samples.json`
- Removed the unused documentation link from `SampleConfiguration.cs`
- [Breaking] Reworked the platorm support implementations with new BasePkcePlatformSupport class

### Removed
- Removed unused private usage of JsonUtility.

## [0.5.0] - 2022-10-05

### Changed
- [Breaking] These interfaces does not inherit from IDisposable anymore: IAppInfoProvider, IHttpClient, IMessagingClient, IScene, IServiceHttpClient.

### Fixed
- Fix Windows custom uri scheme registration on IL2CPP

### Removed
- [Breaking] Removed the PlatformSupportFactory. Use the MessagingClient constructor directly

## [0.4.0] - 2022-09-22

### Added
- IKeyValueStore, FileKeyValueStore, BrowserKeyValueStore and CommonBrowserInterop.
- AsyncUrlRedirectAwaiter, IUrlRedirectAwaiter, IUrlRedirectionInterceptor, UriSchemeRedirection, UrlRedirectionInterceptor, UrlRedirectResult and UrlRedirectStatus.
- PreBuildValidation, BuildUtils, AppLinksHelper, WindowsBuildPostProcess, OSXPlistParser, InfoPlistPostProcessBuild, XCodePostProcessBuild, AndroidBuildPostProcess.

### Changed
- README.md, LICENSE.md, Third Party Notice.md
- [Breaking] Cleaning folder hierarchy and namespaces

### Fixed
- OSX standalone build
- IL2CPP build on Windows

## [0.3.0] - 2022-09-15

### Added
- [Breaking] A new CloudConfiguration parameter to the AppInfoProvider.
- Added a UnityCloudConfigurationFactory for Unity users and a CloudConfigurationFactory for non-Unity users to create CloudConfiguration objects.

### Changed
- Fixed method signatures in IHttpClient and moved DownloadFilePath to ServiceHttpClientOptions

### Removed
- [Breaking] Removed the static CloudConfiguration class and replace it with a non-static one.
- [Breaking] TraceId no longer exposed in ServiceHttpClient and MessagingClient constructors

## [0.2.0] - 2022-09-08

### Added
- Unity.DigitalTwins.AppSettings namespace.
- Scripting reference documentation
- IAppIdProvider and IAppInfoProvider interface.
- Digital Twins "App Registration" entry in Player Settings to fetch and cache registered app information.
- X-Digital-Twins-AppId, X-Digital-Twins-ClientTrace and X-Digital-Twins-Trace Headers in ServiceHttpClient SendAsync method.
- LatestVersion property in IScene.
- DownloadFileAsync method in IHttpClient.

### Changed
- [Breaking] Force GCP Locale region in CloudConfiguration and RegionUtils.
- [Breaking] AppId string constructor arguments replaced by IAppIdProvider in ServiceClientHttp.
- [Breaking] Moved some classes from Unity.DigitalTwins.ServiceClient to new Unity.DigitalTwins.Cloud namespace.
- Refactored DownloadFileToDiskAsync into RequestAsync in LegacyRequestHandler
- Fix to WebGL messaging client
- Prevent build on default App Name value
- Fixing Json API

### Removed
- [Breaking] X-Reflect-ClientTrace, X-Reflect-Trace and X-Reflect-AppId removed from headers in ServiceHttpClient SendAsync method.

## [0.1.0] - 2022-08-26

### Changed
- SceneId and WorkspaceId have been changed from String to System.Guid.

## [0.0.1] - 2022-08-05

### Added
- DT_TRACE environment variable in headers.
- CloudConfiguration, CloudEnvironment, RegionUtils and ProtocolConfiguration.
- LogEvent now has an Exception field that can be added and passed to the ILogOutput implementation.
- The DTLogger class now has overloads for handling an exception object.
- SampleDependencyImporter for automatically importing common asset dependencies alongside a sample

### Changed
- The LogError extension methods now pass the entire Exception method instead of just the message field.

### Removed
- [Breaking] ISceneProvider, IWorkspaceProvider and associated model classes has been moved to the new Storage package.
- [Breaking] Moved the QueryArgumentParser to the HttpClient asmdef.
- [Breaking] Moved the QueryArgumentProcessor, DeepLinkProvider and associated model classes to the new DeepLinking package.
