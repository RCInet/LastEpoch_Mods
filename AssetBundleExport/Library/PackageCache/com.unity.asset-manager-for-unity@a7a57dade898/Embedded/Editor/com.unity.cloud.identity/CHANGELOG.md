# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.4.0] - 2025-06-26

### Changed
- Documentation to reference WebGL support
- `PkceConfigurationProviderFactory`, `ServiceAccountAuthenticator`, `ServiceAccountAuthenticatorSettings`, `ServiceAccountAuthenticatorSettingsBuilder`, `ServiceConnector` and `ServiceConnectorFactory` classes are no longer experimental.
- Experimental `ServiceAccountBase64EncodedCredentials` struct is no longer experimental, is renamed to `ServiceAccountCredentials` and is not `readonly` anymore.
- `ServiceAccountAuthenticatorSettingsBuilder` ctor requires `IHttpClient`, `IServiceHostResolver`, and `IAuthenticationPlatformSupport` parameters.
- `ServiceAccountAuthenticatorSettingsBuilder.AddAppIdProvider` renamed to `ServiceAccountAuthenticatorSettingsBuilder.SetAppIdProvider`.
- `ServiceAccountAuthenticatorSettingsBuilder.AddServiceAccountCredentialsExchanger` renamed to `ServiceAccountAuthenticatorSettingsBuilder.SetServiceAccountCredentialsExchanger`.
- `ServiceAccountAuthenticatorSettingsBuilder.AddDefaultServiceAccountCredentialsExchanger` renamed to `ServiceAccountAuthenticatorSettingsBuilder.SetServiceAccountCredentialsExchanger`.

### Removed
- Experimental `ServiceConnectorFactory.CreateForUnityServicesGateway` method.
- Experimental `PkceConfigurationProviderFactory.CreateForUnityServicesGateway` method.
- Experimental `ServiceAccountAuthenticatorSettingsBuilder.AddAuthenticationPlatformSupport` method.
- Experimental `ServiceAccountAuthenticatorSettingsBuilder.AddHttpClient` method.
- Experimental `ServiceAccountAuthenticatorSettingsBuilder.AddServiceHostResolver` method.

## [1.4.0-exp.2] - 2025-05-12

### Changed
- Introducing documentation for VPC support
- Cleaned up documentation
- Rename FQDN path prefix env var name

### Fixed
- WebGL plaform missing method in ServiceHostResolverFactory
- Prevent UnityEditorAuthorizerSample to run when using Private Services config

## [1.4.0-exp.1] - 2024-12-12

### Added
- `PkceConfigurationProviderFactory`, `ServiceAccountAuthenticator`, `ServiceAccountAuthenticatorSettings`, `ServiceAccountAuthenticatorSettingsBuilder`, `ServiceAccountBase64EncodedCredentials`, `ServiceConnector`, `ServiceConnectorFactory` classes. 

### Changed
- Improve the package directory structure to regroup related classes in `Core`.

### Fixed
- Improve the UnityEditorServiceAuthorizer resilience to domain reloads.

### Deprecated
- `PkceConfiguration`, `PkceConfigurationProvider` and `ServiceAccountAuthorizer` classes.

## [1.3.1] - 2024-10-18

### Changed
- Minor documentation updates

## [1.3.0] - 2024-09-23

### Added
- `IJwtDecoder` interface and `JwtToken` class.
- WebGL platform support.

### Changed
- `PkceAuthenticatorSettingsBuilder` has a new `JwtDecoder` property and a new `AddJwtDecoder` method.
- `UnityEditorServiceAuthorizer.OverrideUnityEditorServiceAuthorizer` method accepts a new `IJwtDecoder` parameter.

### Removed
- EXPERIMENTAL_WEBGL_PROXY compile flag.

## [1.2.1] - 2024-08-26

### Fixed
- `UnityEditorServiceAuthorizer` now activates the Unity Hub to refresh the Unity cloud services token if it was not automatically refreshed in time. 

## [1.2.0] - 2024-05-31

### Added
- [Experimental] WebGL support can be activated using the EXPERIMENTAL_WEBGL_PROXY compile flag.
- `ICloudStorageInfoProvider.HasMeteredBillingActivatedAsync` method.

### Changed
- Experimental `UnityEditorCloudServiceAuthorizer` class was renamed to `UnityEditorServiceAuthorizer` and it now supports transient disconnection for seamless integration in the Unity Editor.
- `ICloudStorageInfoProvider` and `ICloudStorageUsage` interfaces and `CloudStorageUsage` class are no longer experimental.
- `GetCloudStorageUsageAsync` method from `ICloudStorageInfoProvider` now accept an optional CancellationToken.
- `ICloudStorageInfoProvider.GetCloudStorageEntitlementsAsync` method to retrieve experimental `ICloudStorageEntitlement` was removed.
- `IOrganization` inherits `ICloudStorageInfoProvider`.
- `Organization` no longer exposes inherited method `GetCloudStorageEntitlementsAsync`.

### Removed
- `ICloudStorageEntitlements` and `ICloudStorageEntitlement` experimental interfaces.
- `CloudStorageEntitlements` and `CloudStorageEntitlement` experimental classes.

### Deprecated
- `UnityEditorAuthenticator` has been deprecated in favor of the `UnityEditorServiceAuthorizer`.

## [1.2.0-exp.2] - 2024-05-02

### Added
- Added Apple Privacy Manifest documentation.

## [1.2.0-exp.1] - 2024-04-19

### Added
- [Experimental] `ICloudStorageInfoProvider`, `ICloudStorageUsage` `ICloudStorageEntitlements` and `ICloudStorageEntitlement` interfaces.
- [Experimental] `CloudStorageUsage`, `CloudStorageEntitlements` and `CloudStorageEntitlement` classes.
- [Experimental] Add new `UnityEditorCloudServiceAuthorizer` supporting domain reload and transient disconnection in the editor.

### Changed
- [Experimental] `IOrganization` inherits `ICloudStorageInfoProvider`.

## [1.1.0] - 2024-04-05

### Added
- Add `UnityEditorAuthenticator` parameter less default constructor.
- Added Apple Privacy Manifest file to `/Plugins` directory.

### Changed
- Replace internal endpoints usage with public endpoints to support WebGL platform.
- Updated default `PkceConfiguration` to use public endpoints for token management.
- Modified the `LogLevel` for several log messages to reduce the default amount of logs in the console.
- Manual documentation code-snippets set to compile only in editor.

### Fixed
- `CloudProjectSettingsUnityEditorAccessTokenProvider` multithread support.

## [1.0.0] - 2024-02-26

### Added
- Add `IUnityUserInfo`, `IMemberInfo`, `IUnityUserInfoProvider` and `IMemberInfoProvider` interfaces.
- `IOrganization` and `IProject` interfaces now inherits `IMemberInfoProvider` interface.
- `IProject` interface now exposes an `EnabledInAssetManager` boolean property.
- `IAuthenticator` interface and all implementations now inherits `IUnityUserInfoProvider` interface.

### Changed
- Updated Manual Documentation.
- [Breaking] Renamed `IUnityUserInfo` interface to `IUserInfo`.
- [Breaking] Renamed `IUnityUserInfoProvider` interface to `IUserInfoProvider`.
- [Breaking] Renamed `GetUnityUserInfoAsync` method of `IUserInfoProvider` interface to `GetUserInfoAsync`.
- [Breaking] Renamed `GenesisId` property to `UserId` in `IUnityUserInfo` interface.
- [Breaking] Renamed `GroupGenesisId` property to `GroupId` in `IMemberInfo` interface.
- `UnityEditorAuthenticator`, `IUnityEditorAccessTokenProvider` and `LaunchArgumentsUnityEditorAccessTokenProvider` are now public.
- [Breaking] Replaced `IPkceRequestHandler.GetAuthenticatedUserInfoAsync` method with `IPkceRequestHandler.GetUserInfoAsync`.
- [Breaking] Renamed `AuthenticatedUserInfoClaims` to `OpenIdUserInfoClaims`.
- [Breaking] Changed `IOrganization.Role` property type from `string` to `Role`.
- [Breaking] Changed `ListOrganizationsAsync` methods signature of `IOrganizationRepository` interface to return an `IAsyncEnumerable<IOrganization>` and accept `Range` and `CancellationToken` arguments.
- [Breaking] Removed `HasRoleAsync` and `HasPermissionAsync` methods from `IRoleProvider` interface in favor of `IEnumerable<Role>` and `IEnumerable<Permission>` type extensions.
- [Breaking] Moved `UserId` struct to Unity.Cloud.Common.
- Update com.unity.cloud.common dependency to 1.0.0.

### Removed
- [Breaking] Removed `Id` property in `IUnityUserInfo` interface.
- [Breaking] Removed `Coppa`, `KidsStoreCompliance` and `DefaultEnvironmentId` properties from `IProject` interface and its implementation.
- [Breaking] Removed `LicenseInfo` class and `LicenseType` enum.
- [Breaking] Removed `IAuthenticatedUserInfoProvider` interface and its implementation in all `IAuthenticator`.
- [Breaking] Renamed `AuthenticatorOrganizationRepository` to `AuthenticatedUserSession` and set as internal. 

## [1.0.0-pre.6] - 2024-02-13

### Added
- Add `GetOrganizationAsync` method in `IOrganizationRepository` interface.
- Added missing await operator in the `UserNameUpdater` class.

### Changed
- Update com.unity.cloud.common dependency to 1.0.0-pre.6.

## [1.0.0-pre.5] - 2024-01-15

### Added
- [Breaking] `ChannelServiceRequest`, `ChannelRequest`, `ChannelPkcePlatformSupport`, `ChannelInfo` from Unity.Cloud.Interop to Unity.Cloud.Identity.

### Changed
- [Breaking] Moved `IChannelProvider`, `ChannelProvider`, `ChannelJson`, `ChannelIdResponseJson` from Unity.Cloud.Interop to Unity.Cloud.Identity and maked them internal.
- Update com.unity.cloud.common dependency to 1.0.0-pre.5.

## [1.0.0-pre.4] - 2023-12-20

### Changed
- Migrated interoperability classes from Common to Identity.

### Removed
- [Breaking] Removed `AppInfo`, `AppInfoProvider`, `IAppInfoProvider` and `UnityCloudAppRegistration`.

## [1.0.0-pre.3] - 2023-12-07

### Added
- Added `UnityCloudPlayerSettings` class and unit tests.

### Changed
- [Breaking] Moved code causing circular dependency from Common/Editor/Settings and Common/Editor/BuildTools to Identity/Editor.
- Removed remaining hard-coded domain host validation in `BrowserAuthenticatedAccessTokenProvider`.
- Setter of `UnityServicesToken.AccessToken` property is now public.
- UCLogger's LogInfo extension method was renamed to LogInformation.

### Removed
- [Breaking] Removed `IAppNameProvider` and `IAppDisplayNameProvider` references in `PkceAuthenticatorSettingsBuilder`, `CompositeAuthenticatorSettingsBuilder`, `UnityEditorAuthenticator`, `PkceAuthenticator` and all classes derived from `BasePkcePlatformSupport`.
- [Breaking] Removed `AppName` property of `PkceConfiguration`.

## [1.0.0-pre.2] - 2023-11-23

### Changed
- Improved Get User Info sample
- Switched from multiple asmef files to one asmdef file in Samples/Shared and multiple asmref files in each sample folder.
- Added the !UC_EXCLUDE_SAMPLES constraint to the asmdef file in Samples/Shared and removed the conditional compilation code in the sample scripts.
- [Breaking] Removing domain host validation in `BrowserAuthenticatedAccessTokenProvider`.

## [1.0.0-pre.1] - 2023-11-07

### Changed
- Improved information in README
- Move app registration documentation to the Common package.
- Updated LICENSE.md file.
- Improved manual documentation and scripting API.

## [1.0.0-exp.1] - 2023-10-26

### Added
- `IOrganization` and `IProject` can now list their respective roles and permissions.

### Changed
- Change minimal Unity version to 2022.3

### Fixed
- `UnityEditorAuthenticator` now exchanges the `CloudProjectSettings.accessToken` for a Unity Cloud Services token when it's value has been internally refreshed by the Unity Editor.
- `PkceAuthenticator` no longer empties the portalAccessToken cookie on a logout operation on WebGL.

### Removed
- [Breaking] Removing obsolete constructors for `PkceAuthenticator`.
- [Breaking] Removing `CommandLineAccessTokenProvider`.
- [Breaking] Removing obsolete `IOrganization.ListProjectsAsync()`.
- [Breaking] Removing obsolete `PkceAuthenticator.DeviceTokenRefreshed` event.

## [0.21.0] - 2023-10-18

### Added
- `AddDefaultPkceAuthenticator` and `AddDefaultBrowserAuthenticatedAccessTokenProvider` methods of the `CompositeAuthenticatorSettingsBuilder` require an additional `IAppNamespaceProvider` parameter.
- `AddAppNamespaceProvider` method to the `PkceAuthenticatorSettingsBuilder` class.
- `AddDefaultConfigurationProviderAndRequestHandler` method of `PkceAuthenticatorSettingsBuilder` class requires an additional `IAppNamespaceProvider` parameter.
- `IOrganization`, `IProject`, `IRoleProvider` and `IOrganizationRespository` interfaces.
- [Breaking] `IAuthenticator` implements `IOrganizationRespository`.
- [Breaking] `CompositeAuthenticatorSettingsBuilder` requires an additional `IAppIdProvider` argument.
- Added the `ServiceAccountAuthorizer` implementation to support service account authentication.
- Added the `UnityServicesDomainResolver` implementation to support internal endpoints.

### Changed
- `BasePkcePlatformSupport` and all its derived classes require an additional `IAppNamespaceProvider` parameter in the constructor method. 
- `ListProjectsAsync` in `IOrganization` is now returning an `IAsyncEnumerable<Iproject>` to support paging.
- [Breaking] `HttpMetadataDataSource` now expects `IServiceHttpClient` instead of `IHttpClient`.
- `BasePkcePlatformSupport` and all its derived classes require an additional `IAppNamespaceProvider` parameter in the constructor method. 
- `ListProjectsAsync` in `IOrganization` is now returning an `IAsyncEnumerable<Iproject>` to support paging.
- [Breaking] `HttpMetadataDataSource` now expects `IServiceHttpClient` instead of `IHttpClient`.

### Removed
- `Azure` and `GCP` service provider no longer supported.
- `Azure` and `GCP` service provider no longer supported.
- [Breaking] Removed `PersonalAccessTokenProvider`.
- `IUserInfoProvider` and `UserInfoProvider` have been removed, replaced by `IAuthenticatedUserInfoProvider` and `AuthenticatedUserInfoProvider`.
- Removed all dependencies to `com.unity.cloud.storage`.

### Deprecated
- `ListProjectsAsync` returning an `Task<IEnumerable<IProject>>` has been deprecated and will be removed in a future release.

## [0.20.0] - 2023-09-15

### Added
- `IAuthenticatedUserInfo` and `IAuthenticatedUserInfoProvider` interfaces.
- `AuthenticatedUserInfoClaims` class.
- `UserInfoUrl` property in `PkceConfiguration`.
- [Breaking] `BrowserAuthenticatedAccessTokenProvider` and `CommandLineAccessTokenProvider` use `PkceAuthenticatorSettings` injection in the constructor.
- [Breaking] `IAuthenticator` implements `IAuthenticatedUserInfoProvider`.
- [Breaking] `IAccessTokenProvider` has been renamed to `IServiceAuthorizer` and applies authorization information directly to HTTP requests.

### Deprecated
- `PersonalAccessTokenProvider` has been deprecated and will be removed in a future release.

## [0.19.0] - 2023-08-31

### Added
- Added official support for the latest LTS Editor 2022.3 while maintaining support for 2021.3.
- [Breaking] Added ClientId and UserId classes to replace string IDs.

### Changed
- Default `PkceConfiguration` TokenUrl, RefreshTokenUrl and LogoutUrl no longer use cloud service endpoints.
- [Breaking] WebGL platform use secure document.cookie instead of HTTP headers to reach cloud service endpoints.

## [0.18.1] - 2023-08-17

### Fixed
- `BasePkcePlatformSupport` and `IosPkcePlatformSupport` missing `Application.absoluteURL` injection since version 0.18.0.

## [0.18.0] - 2023-07-20

### Added
- [Breaking] `GetRedirectUriAsync` method in `IAuthenticationPlatformSupport` interface and all derived classes.
- Added `PkceAuthenticatorSettings` and `PkceAuthenticatorSettingsBuilder` to simplify the creation of `PkceAuthenticator` instances.

### Changed
- [Breaking] `PlatformSupportFactory.GetAuthenticationPlatformSupport` static method accept additional `IAppIdProvider`, `IAppNameProvider`, `cacheStorePath` parameters.
- [Breaking] `BasePkcePlatformSupport` have been moved to `Unity.Cloud.Identity` namespace.
- [Breaking] `BasePkcePlatformSupport` constructor and derived classes constructor accept additional `IUrlProcessor`, `IAppIdProvider`, `IAppNameProvider`, `cacheStorePath` and `activationUrl` parameters.
- Deprecated `PkceAuthenticator` constructors that don't accept a `PkceAuthenticatorSettings` instance.

## [0.17.0] - 2023-07-07

### Changed
- [Breaking] Uses of `ServiceHostConfiguration` have been replaced for `IServiceHostResolver`.  
- Updated all references to Common's `IHttpClient.SendAsync` to match its new signature.

### Fixed
- The Authentication sample now correctly displays an error when the provided Personal Access Token is invalid.

## [0.16.1] - 2023-06-22

### Added
- `IAccessTokenExchanger` interface.
- `UnityServicesToken`, `ExchangeGenesisTokenRequest`, `TargetClientIdToken`, `ExchangeGenesisAccessTokenResponse`, `ExchangeTargetClientIdTokenResponse` class.
- `DeviceTokenToUnityServicesTokenExchanger` and `TargetClientIdTokenToUnityServicesTokenExchanger` implementation of the `IAccessTokenExchanger` interface.
- `UnityEditorAuthenticator` class to return a `UnityServicesToken` from the user logged in the Unity Editor.

### Changed
- `PkceAuthenticator` returns an access token from a `UnityServicesToken` in its `IAccessTokenProvider` implementation.
- `PkceAuthenticator` constructors not supporting the injection of an `IAccessTokenExchanger` implementation class marked as obsolete.
- `CompositeAuthenticatorSettingsBuilder.AddDefaultPkceAuthenticator` marked as obsolete in favor of a new override that accepts only an `IAppNameProvider` argument.
- `PkceConfigurationProvider` constructor marked as absolete in favor of a new constructor that accepts `ServiceHostConfiguration` and `IAppNameProvider` arguments.

## [0.16.0] - 2023-05-25

### Added
- Proxy redirect routes exposed in `PkceConfiguration`.

### Changed
- Device tokens are now saved per `ServiceEnvironment`.
- [Breaking] `ServiceHostConfiguration` is now required to build a `PkceConfigurationProvider` and `CompositeAuthenticatorSettingsBuilder`.
- [Breaking] `PkceConfigurationProvider` will build a `PkceConfiguration` dynamically based on the `ServiceHostConfiguration` provided.
- `CompositeAuthenticator` in samples now takes `ServiceHostConfiguration` as a parameter.

### Removed
- [Breaking] `DefaultConfiguration` has been removed from `PkceConfiguration`. `PkceConfigurationProvider` will build a default configuration based on a provided `ServiceHostConfiguration`.

## [0.15.3] - 2023-05-11

### Added
- Exposed an event in `PkceAuthenticator.cs` which is raised whenever the device token is refreshed.

### Changed
- Upgrade to Moq 2.0.0-pre.2

## [0.15.2] - 2023-04-27

### Added
- Added a documentation section on `Managed Stripping Level` for build settings.

### Fixed
- Reload browser page issue on WebGL.

## [0.15.1] - 2023-04-13

### Added
- `SignOutUrl` property in `PkceConfiguration`.
- `IAuthenticationPlatformSupport.GetRedirectUri` method accepts an optional string parameter to support any redirection route.

### Changed
- `LogoutAsync` method in `IUrlRedirectionAuthenticator` interface and its derived implementations accepts an optional boolean value to clear the browser cache.

## [0.15.0] - 2023-03-30

### Added
- Added Api version info to package

### Changed
- [Breaking] Change identifier types from Guid to SceneId, WorkspaceId, DatasetId, OrganizationId and VersionId.

### Fixed
- Disable code using Moq when the package is not present.

## [0.14.0] - 2023-03-16

### Changed
- `Authentication` and `GetUserInfo` samples UI refactor.
- [Breaking] All mentions of "Digital Twins" and "DT" renamed to their "Unity Cloud" equivalent, or removed altogether

## [0.13.0] - 2023-03-02

### Added
- New `CompositeAuthenticatorSettings` and `CompositeAuthenticatorSettingsBuilder`.
- New constructor for `CompositeAuthenticator` accepting a `CompositeAuthenticatorSettings` instance.

### Removed
- [Breaking] the `CompositeAuthenticator` constructor that accepts a list of `IAuthenticator`.

## [0.12.1] - 2023-02-24

### Changed
- Update to latest common dependency

## [0.12.0] - 2023-02-16

### Added
- New `CompositeAuthenticatorSettings` and `CompositeAuthenticatorSettingsBuilder`.
- New constructor for `CompositeAuthenticator` accepting a `CompositeAuthenticatorSettings` instance.
- Support PKCE authentication flow for `https://` hosted WebGL builds.

### Changed
- `EditorPkcePlatformSupport` awaits login response from the browser at a randomly attributed port, instead of the fixed 3000 port.
- [Breaking] `ICompositeAuthenticator.Interactive` property renamed to `ICompositeAuthenticator.RequiresGUI`.
- `BrowserAuthenticatedAccessTokenProvider` constructor requires a new `Dictionary<string, string>` to support different host locations.
- [Breaking] `IAuthenticator.HasValidPreconditions` method renamed to `IAuthenticator.HasValidPreconditionsAsync` and returns a `Task<bool>`.
- [Breaking] `IAuthenticationPlatformSupport.OpenUrlAndWaitForRedirection` can now throw a `TimeoutException`.
- Updated tests for new exceptions thrown by url-redirection.

## [0.11.0] - 2023-02-02

### Added
- `ICompositeAuthenticator` interface.

### Changed
- [Breaking] `IUrlRedirectionPlatformSupport` renamed to `IAuthenticationPlatformSupport`.
- `PersonalAccessTokenProvider` and  `CommandLineAccessTokenProvider` no longer blocks the activationUrl consumption flow.
- [Breaking] `PersonalAccessTokenProvider` expects a mandatory `IAuthenticationPlatformSupport` argument.
- [Breaking] `IUrlRedirectionInterceptor` removed from `PkceAuthenticator` constructors.
- [Breaking] `IPkcePlatformSupport` renamed to `IUrlRedirectionPlatformSupport`.
- [Breaking] `IInteractiveAuthenticator` renamed to `IUrlRedirectionAuthenticator`.
- [Breaking] `CompositeAuthenticator` inherits from new `ICompositeAuthenticator` interface.
- [Breaking] `CompositeAuthenticator` constructor has unique mandatory `List<IAuthenticator>` argument.
- [Breaking] `PersonalAccessTokenProvider`,  `CommandLineAccessTokenProvider`, `BrowserAuthenticatedAccessTokenProvider` inherits from `IAuthenticator` interface.
- [Breaking] `ICompositeAuthenticator` and `IAuthenticator` interfaces refactored.

### Removed
- [Breaking] `PreAuthenticatedHostAccessTokenProvider` class.

## [0.10.0] - 2023-01-19

### Changed
- [Breaking] `BasePkcePlatformSupport` uses new `AesStringObfuscator` from common package to encrypt and decrypt the refresh token.

### Removed
- [Breaking] `EditorActivateFromUrl` runtime class.

## [0.9.1] - 2022-12-08

### Changed
- Manage App Tracking permissions on iOS when logging in captive Safari controller.
- Removed warnings in package

## [0.9.0] - 2022-11-24

### Added
- Support login cancellation from url.
- GetCancellationUri method in IPkcePlatformSupport and all derived platform specific implementations.
- CancelLogin method in IAuthenticator, PkceAuthenticator and CompositeAuthenticator.
- IAuthenticator's AuthenticationState property and AuthenticationStateChange event moved to a separate new IAuthenticationStateProvider.
- All derived IAccessTokenProvider classes implements additional new IAuthenticationStateProvider.
- [Breaking] InitializeAsync public method in PreAuthenticatedHostAccessTokenProvider, BrowserAuthenticatedAccessTokenProvider, CommandLineAccessTokenProvider and PersonalAccessTokenProvider.
- AuthenticationState.AwaitingInitialization enum value.
- BasePkcePlatformSupport.

### Changed
- Fix QueryArgumentHandler<float> registration issue in QueryArgumentsProcessor.Register().
- Manual documentation updates
- [Breaking] Fix PkceAuthenticator constructor override missing required IHttpClient value.
- Renamed the samples' `Common` directory to `Shared`
- Updated the naming convention for the sample configuration and removed the doc-link field

### Removed
- Internal UnitySynchronizationContextGrabber class.
- [Breaking] LaunchArgumentsParser has moved to the com.unity.digital-twins.common package.
- LinuxPkcePlatformSupport, OsxPlatformSupport and AndroidPlatformSupport.
- PlatformSupportFactory.GetActivatePlatformSupport method.

## [0.8.0] - 2022-10-05

### Added
- Support activation from url in playmode using EditorActivateFromUrl monobehaviour.
- EditorActivateFromUrl component to mock deep link

### Changed
- [Breaking] These interfaces does not inherit from IDisposable anymore: IAuthenticator, IInteractiveAuthenticator, IPkceConfigurationProvider, IPkcePlatformSupport, IUserInfoProvider.
- Manual documentation and samples improvements

## [0.7.0] - 2022-09-22

### Changed
- [Breaking] Removed DeepLinkActivated event in IAuthenticator and IActivatePlatformSupport.
- [Breaking] Added a required IUrlRedirectionInterceptor argument in PkceAuthenticator constructors.
- README.md, LICENSE.md.
- Manual documentation improvements
- [Breaking] Support caching and resuming of an activationUrl in WebglActivatePlatformSupport and PkceAuthenticator.

### Fixed
- WebGL login redirection
- OSX standalone build

### Removed
- ICacheStore, FileCacheStore, WebGLCacheStore and BrowserHostInterop.
- AsyncUrlRedirectAwaiter, IUrlRedirectAwaiter, IUrlRedirectionInterceptor, UriSchemeRedirection, UrlRedirectionInterceptor, UrlRedirectResult and UrlRedirectStatus.
- PreBuildValidation, BuildUtils, AppLinksHelper, WindowsBuildPostProcess, OSXPlistParser, InfoPlistPostProcessBuild, XCodePostProcessBuild, AndroidBuildPostProcess.

## [0.6.0] - 2022-09-15

### Added
- BrowserAuthenticatedAccessTokenProvider.

### Changed
- [Breaking] Removed a PkceAuthenticator constructor overload.
- [Breaking] Added a CloudConfiguration to the UserInfoProvider constructor.

## [0.5.0] - 2022-09-08

### Added
- IInteractiveAuthenticator, IPreAuthenticatedAccessTokenProvider, IPkceConfigurationProvider and IUserInfoProvider interface.
- Added manual documentation to package
- Added samples and samples documentation to package

### Changed
- Use dt.unity.com domain hosted proxy page for login in browser.
- [Breaking] UserInfoProvider constructor has new single IServiceHttpClient argument.
- [Breaking] Renamed GetPkceConfiguration method to GetPkceConfigurationAsync in IPkceConfigurationProvider.
- [Breaking] PkceAuthenticator constructor now requires IPkcePlatformSupport, IAppIdProvider and IAppNameProvider.
- [Breaking] Renamed all directory matching namespaces.
- [Breaking] AppConfiguration constructor arguments replaced with IPkceConfigurationProvider in IAuthenticator implementations.
- [Breaking] AppConfiguration renamed to PkceConfiguration.
- [Breaking] Changed parameter UNITY_DT_PERSONAL_ACCESS_TOKEN and UNITY_DT_ACCESS_TOKEN to DT_PERSONAL_ACCESS_TOKEN and DT_ACCESS_TOKEN

### Removed
- [Breaking] AccessTokenProviderFactory, UserInfoProviderFactory.

## [0.4.0] - 2022-08-26

- Use CloudConfiguration static method to retrieve base host address for Cloud endpoints and support DT_CLOUD override from environment variable.
- `IdentityPlayerSettingsProvider` now attempts to resource-load existing settings rather than searching at one specific path.



## [0.3.0] - 2022-08-05

### Added
- Prefix to custom uri scheme.
- Support for WebGL deep link consumption.

### Changed
- [Breaking] DeepLinkActivated method signature modified in IActivatePlaformSupport.

## [0.2.0] - 2022-07-18

Add Personal Access Token and pre-authenticated host support.



## [0.1.1] - 2022-06-08

Fix bug with ActivationUrl found in release 0.1.0.



## [0.1.0] - 2022-06-07

Initial package on Artifactory. Changes are not listed here yet.



## [0.0.1] - 2022-06-01

Initial Release
