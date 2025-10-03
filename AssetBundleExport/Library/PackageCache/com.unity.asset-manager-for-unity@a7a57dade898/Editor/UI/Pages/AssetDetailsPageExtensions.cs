using System;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    delegate Task<UserChip> CreateUserChip(string userId, Type searchFilterType);

    delegate ProjectChip CreateProjectChip(string projectId);

    static class AssetDetailsPageExtensions
    {
        public static string GetImportButtonLabel(BaseOperation operationInProgress, AssetPreview.IStatus status)
        {
            var isImporting = operationInProgress?.Status == OperationStatus.InProgress;

            if (isImporting)
            {
                return $"{L10n.Tr(Constants.ImportingText)} ({operationInProgress.Progress * 100:0.#}%)";
            }

            return status != null && !string.IsNullOrEmpty(status.ActionText) ? L10n.Tr(status.ActionText) : L10n.Tr(Constants.ImportActionText);
        }

        public static string GetImportButtonTooltip(BaseOperation operationInProgress, UIEnabledStates enabled)
        {
            var isEnabled = operationInProgress?.Status != OperationStatus.InProgress
                && enabled.HasFlag(UIEnabledStates.HasPermissions)
                && enabled.HasFlag(UIEnabledStates.ServicesReachable);

            if (isEnabled)
            {
                if ( enabled.HasFlag(UIEnabledStates.InProject) )
                {
                    return L10n.Tr(Constants.ReimportButtonTooltip);
                }
                return L10n.Tr(Constants.ImportButtonTooltip);
            }

            if (!enabled.HasFlag(UIEnabledStates.ServicesReachable))
            {
                return L10n.Tr(Constants.UploadCloudServicesNotReachableTooltip);
            }

            if (!enabled.HasFlag(UIEnabledStates.HasPermissions))
            {
                return L10n.Tr(Constants.ImportNoPermissionMessage);
            }

            return enabled.HasFlag(UIEnabledStates.IsImporting) ? string.Empty : L10n.Tr(Constants.ImportButtonDisabledToolTip);
        }

        public static void AddProjectChip(this CreateProjectChip createProjectChip, VisualElement container, params string[] projectIds)
        {
            container.Clear();

            foreach (var projectId in projectIds)
            {
                var projectChip = createProjectChip?.Invoke(projectId);

                if (projectChip != null)
                {
                    container.Add(projectChip);
                }
            }

            UIElementsUtils.SetDisplay(container.parent, container.childCount > 0);
        }

        public static async Task AddUserChip(this CreateUserChip createUserChip, VisualElement container, string userId, Type searchFilterType)
        {
            container.Clear();

            var userChip = await (createUserChip?.Invoke(userId, searchFilterType) ?? Task.FromResult((UserChip) null));

            UIElementsUtils.SetDisplay(container, userChip != null);

            if (userChip != null)
            {
                container.Add(userChip);
            }
        }

        public static UIEnabledStates GetFlag(this UIEnabledStates flag, bool value)
        {
            return value ? flag : UIEnabledStates.None;
        }

        public static bool IsImportAvailable(this UIEnabledStates enabled)
        {
            var isEnabled = !enabled.HasFlag(UIEnabledStates.IsImporting)
                && enabled.HasFlag(UIEnabledStates.CanImport)
                && enabled.HasFlag(UIEnabledStates.HasPermissions)
                && enabled.HasFlag(UIEnabledStates.ServicesReachable)
                && enabled.HasFlag(UIEnabledStates.ValidStatus);

            return isEnabled;
        }
    }
}
