using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public static readonly string ProgressBarError = "progress-bar--error";
        public static readonly string ProgressBarSuccess = "progress-bar--success";
        public static readonly string ProgressBarWarning = "progress-bar--warning";
        public static readonly string ProgressBarGridItem = "grid-view--item-download_progress_bar";
        public static readonly string ProgressBarDetailsPage = "details-page-progress-bar";
        public static readonly string ProgressBarDetailsPageContainer = "details-page-download-progress-container";
        public static readonly string ProgressBarDetailsPageCancelButton = "details-page-download-cancel-button";
    }

    class OperationProgressBar : ProgressBar
    {
        public OperationProgressBar(Action cancelCallback = null): base(cancelCallback)
        {
            if (cancelCallback == null)
            {
                m_ProgressBarContainer.AddToClassList(UssStyle.ProgressBarGridItem);
            }
            else
            {
                AddToClassList(UssStyle.ProgressBarDetailsPageContainer);
                AddToClassList(UssStyle.ProgressBarDetailsPage);

                m_CancelButton.AddToClassList(UssStyle.ProgressBarDetailsPageCancelButton);
            }
        }

        internal void Refresh(BaseOperation operation)
        {
            if (operation == null)
            {
                Hide();
                return;
            }

            switch (operation.Status)
            {
                case OperationStatus.Success:
                    m_IsFinished = true;
                    m_ProgressBar.AddToClassList(UssStyle.ProgressBarSuccess);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarColor);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarError);
                    break;

                case OperationStatus.Error:
                    m_IsFinished = true;
                    m_ProgressBar.AddToClassList(UssStyle.ProgressBarError);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarColor);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarSuccess);
                    break;

                case OperationStatus.Paused:
                    m_CancelButton?.SetEnabled(false);
                    m_ProgressBar.AddToClassList(UssStyle.ProgressBarWarning);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarColor);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarSuccess);
                    break;
                case OperationStatus.Cancelled:
                    m_IsFinished = true;
                    m_ProgressBar.AddToClassList(UssStyle.ProgressBarWarning);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarColor);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarSuccess);
                    break;
                default:
                    m_ProgressBar.AddToClassList(UssStyle.ProgressBarColor);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarError);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarSuccess);
                    m_ProgressBar.RemoveFromClassList(UssStyle.ProgressBarWarning);
                    break;
            }

            if (operation.Status == OperationStatus.InProgress)
            {
                m_CancelButton?.SetEnabled(true);
                SetProgress(operation.Progress);
            }
            else
            {
                if (operation.IsSticky || operation.Status == OperationStatus.Error || operation.Status == OperationStatus.Cancelled)
                {
                    m_IsFinished = true;
                    UpdateTooltip();
                    SetProgress(1.0f);
                }
                else if(operation.Status == OperationStatus.Paused)
                {
                    if (m_IsIndefinite)
                    {
                        SetProgress(1.0f);
                    }
                }
                else
                {
                    Hide();
                }
            }
        }

        protected override void UpdateTooltip()
        {
            if (m_CancelButton != null)
            {
                m_CancelButton.tooltip = m_IsFinished ?  L10n.Tr(Constants.ClearImportActionText) : L10n.Tr(AssetManagerCoreConstants.CancelImportActionText);
            }
        }
    }
}
