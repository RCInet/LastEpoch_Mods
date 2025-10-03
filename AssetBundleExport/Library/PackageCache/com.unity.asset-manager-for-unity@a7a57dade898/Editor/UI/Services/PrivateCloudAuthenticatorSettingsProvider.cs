using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string ProjectSettingsRoot = "project-settings-root";
        public const string ProjectSettingsTitle = "project-settings-title";
        public const string ProjectSettingsHelpBox = "project-settings-help-box";
        public const string ProjectSettingsContainer = "project-settings-container";
        public const string ProjectSettingsField = "project-settings-field";
        public const string ProjectSettingsButton = "project-settings-button";
    }

    /// <summary>
    /// A <see cref="SettingsProvider"/> implementation for Unity Cloud settings.
    /// </summary>
    class PrivateCloudAuthenticatorSettingsProvider : SettingsProvider
    {
        const string k_MainDarkUssName = "MainDark";
        const string k_MainLightUssName = "MainLight";

        HelpBox m_HelpBox;
        TextField m_DomainNameField;
        TextField m_PathPrefixField;
        TextField m_ManifestUrlField;
        Button m_ConnectionButton;
        Button m_LoginButton;
        TextField m_OrganizationField;
        TextField m_ProjectField;

        PrivateCloudAuthenticatorSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        /// <summary>
        /// Creates an instance of <see cref="PrivateCloudAuthenticatorSettingsProvider"/>.
        /// </summary>
        /// <returns>The created instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreatePrivateCloudAuthenticatorSettingsProvider()
        {
            return new PrivateCloudAuthenticatorSettingsProvider("Project/Asset Manager/Private Cloud Services", SettingsScope.Project)
            {
                keywords = new[]
                {
                    "Private Cloud Services Authentication",
                }
            };
        }

        /// <inheritdoc/>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            UIElementsUtils.LoadCommonStyleSheet(rootElement);
            UIElementsUtils.LoadCustomStyleSheet(rootElement, EditorGUIUtility.isProSkin ? k_MainDarkUssName : k_MainLightUssName);

            rootElement.AddToClassList(UssStyle.ProjectSettingsRoot);

            var permissionsManager = ServicesContainer.instance.Resolve<IPermissionsManager>();
            if (permissionsManager == null)
            {
                rootElement.Add(new HelpBox
                {
                    messageType = HelpBoxMessageType.Error,
                    text = "Failed to initialize AssetManager services."
                });
                return;
            }

            var title = new Label("Private Cloud Services Authentication");
            title.AddToClassList(UssStyle.ProjectSettingsTitle);

            var settings = PrivateCloudSettings.Load();

            permissionsManager.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            permissionsManager.AuthenticationStateChanged += OnAuthenticationStateChanged;

            // Add help box
            m_HelpBox = new HelpBox
            {
                messageType = HelpBoxMessageType.Info,
            };
            m_HelpBox.AddToClassList(UssStyle.ProjectSettingsHelpBox);
            SetHelpBoxMessage(settings, permissionsManager.AuthenticationState);

            // Add toggle button and container
            var settingsContainer = new VisualElement
            {
                style = {display = settings.ServicesEnabled ? DisplayStyle.Flex : DisplayStyle.None}
            };
            settingsContainer.AddToClassList(UssStyle.ProjectSettingsContainer);

            var toggle = new Toggle
            {
                text = L10n.Tr("Enable Private Cloud Services"),
            };
            toggle.AddToClassList(UssStyle.ProjectSettingsField);
            toggle.SetValueWithoutNotify(settings.ServicesEnabled);
            toggle.RegisterValueChangedCallback(evt =>
            {
                PrivateCloudSettings.SetEnabled(evt.newValue);
                settingsContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            // Add settings fields
            m_DomainNameField = new TextField("Fully Qualified Domain Name")
            {
                value = settings.FullyQualifiedDomainName,
                isDelayed = true
            };
            m_DomainNameField.AddToClassList(UssStyle.ProjectSettingsField);
            m_DomainNameField.RegisterValueChangedCallback(_ => ValidateConfiguration(permissionsManager));

            m_PathPrefixField = new TextField("Path Prefix")
            {
                value = settings.PathPrefix,
                isDelayed = true
            };
            m_PathPrefixField.AddToClassList(UssStyle.ProjectSettingsField);
            m_PathPrefixField.RegisterValueChangedCallback(_ => ValidateConfiguration(permissionsManager));

            m_ManifestUrlField = new TextField("OpenID Manifest URL")
            {
                value = settings.OpenIdManifestUrl,
                isDelayed = true
            };
            m_ManifestUrlField.AddToClassList(UssStyle.ProjectSettingsField);
            m_ManifestUrlField.RegisterValueChangedCallback(_ => ValidateConfiguration(permissionsManager));

            m_OrganizationField = new TextField("[optional] Linked Organization Id")
            {
                value = settings.SelectedOrganizationId,
                isDelayed = true
            };
            m_OrganizationField.AddToClassList(UssStyle.ProjectSettingsField);

            m_ProjectField = new TextField("[optional] Linked Project Id")
            {
                value = settings.SelectedProjectId,
                isDelayed = true
            };
            m_ProjectField.AddToClassList(UssStyle.ProjectSettingsField);

            // Add connection button
            m_ConnectionButton = new Button();
            m_ConnectionButton.AddToClassList(UssStyle.ProjectSettingsButton);
            SetConnectionButtonText(permissionsManager.AuthenticationState);
            m_ConnectionButton.clicked += () => PrivateCloudSettings.EstablishConnection(
                m_DomainNameField.text,
                m_PathPrefixField.text,
                m_ManifestUrlField.text);

            // Add login button
            m_LoginButton = new Button();
            m_LoginButton.AddToClassList(UssStyle.ProjectSettingsButton);
            SetLoginButtonText(permissionsManager.AuthenticationState);
            m_LoginButton.clicked += async () =>
            {
                PrivateCloudSettings.SetLinkedOrganizationAndProject(m_OrganizationField.text, m_ProjectField.text);
                await permissionsManager.AuthenticationStateMoveNextAsync();
            };

            // Populate the root element
            rootElement.Add(title);
            rootElement.Add(m_HelpBox);
            rootElement.Add(toggle);
            rootElement.Add(settingsContainer);
            settingsContainer.Add(m_DomainNameField);
            settingsContainer.Add(m_PathPrefixField);
            settingsContainer.Add(m_ManifestUrlField);
            settingsContainer.Add(m_ConnectionButton);
            settingsContainer.Add(m_OrganizationField);
            settingsContainer.Add(m_ProjectField);
            settingsContainer.Add(m_LoginButton);
        }

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            var settings = PrivateCloudSettings.Load();

            SetHelpBoxMessage(settings, state);
            SetConnectionButtonText(state);
            SetLoginButtonText(state);

            if (state == AuthenticationState.LoggedIn)
            {
                m_OrganizationField.SetValueWithoutNotify(settings.SelectedOrganizationId);
                m_ProjectField.SetValueWithoutNotify(settings.SelectedProjectId);
            }
        }

        void SetConnectionButtonText(AuthenticationState state)
        {
            var buttonText = state switch
            {
                AuthenticationState.LoggedIn => "Update Connection",
                _ => "Establish Connection"
            };

            if (m_ConnectionButton != null)
            {
                m_ConnectionButton.text = L10n.Tr(buttonText);
                m_ConnectionButton.SetEnabled(HasValidAuthenticationConfiguration());
            }
        }

        void SetLoginButtonText(AuthenticationState state)
        {
            var buttonText = state switch
            {
                AuthenticationState.AwaitingInitialization => "Awaiting connection...",
                AuthenticationState.LoggedIn => "Logout",
                AuthenticationState.AwaitingLogin => "Cancel",
                _ => "Login"
            };

            if (m_LoginButton != null)
            {
                m_LoginButton.text = L10n.Tr(buttonText);
                m_LoginButton.SetEnabled(state != AuthenticationState.AwaitingInitialization && HasValidAuthenticationConfiguration());
            }
        }

        void SetHelpBoxMessage(PrivateCloudSettings settings, AuthenticationState state)
        {
            string infoMessage;
            if (!settings.ServicesEnabled)
            {
                infoMessage = "Enable Private Cloud Services to allow Asset Manager to use Private Cloud data.";
            }
            else if (state is AuthenticationState.AwaitingInitialization or AuthenticationState.LoggedOut)
            {
                infoMessage = "Fill in the Private Cloud Services settings and click login to connect to a Private Cloud Services provider.";
            }
            else
            {
                infoMessage = state == AuthenticationState.AwaitingLogin
                    ? "Awaiting authentication operation in the browser..."
                    : "Click the logout button to close your session on the Private Cloud.";
            }

            if (m_HelpBox != null)
            {
                m_HelpBox.text = L10n.Tr(infoMessage);
            }
        }

        void ValidateConfiguration(IPermissionsManager permissionsManager)
        {
            SetConnectionButtonText(permissionsManager.AuthenticationState);
            SetLoginButtonText(permissionsManager.AuthenticationState);
        }

        bool HasValidAuthenticationConfiguration()
        {
            return !string.IsNullOrEmpty(m_DomainNameField.text) &&
                !string.IsNullOrEmpty(m_PathPrefixField.text) &&
                !string.IsNullOrEmpty(m_ManifestUrlField.text) &&
                Uri.TryCreate(m_ManifestUrlField.text, UriKind.Absolute, out _);
        }
    }
}
