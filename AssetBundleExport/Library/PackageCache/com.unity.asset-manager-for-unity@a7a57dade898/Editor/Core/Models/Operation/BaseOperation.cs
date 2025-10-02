using System;
using UnityEditor;

namespace Unity.AssetManager.Core.Editor
{
    enum OperationStatus
    {
        None,
        InProgress,
        Success,
        Cancelled,
        Error,
        Paused
    }

    [Serializable]
    abstract class AssetDataOperation : BaseOperation
    {
        public abstract AssetIdentifier Identifier { get; }
        public override bool ShowInBackgroundTasks => true;
    }

    abstract class BaseOperation
    {
        int m_ProgressId;

        public event Action<OperationStatus> Finished;

        public event Action<float> ProgressChanged;

        public abstract float Progress { get; }

        public abstract string OperationName { get; }

        public abstract string Description { get; }

        public virtual bool StartIndefinite => false;

        public virtual bool IsSticky => false;
        public virtual bool ShowInBackgroundTasks => false;

        public OperationStatus Status { get; private set; } = OperationStatus.None;

        public virtual void Start()
        {
            Status = OperationStatus.InProgress;

            if (ShowInBackgroundTasks)
            {
                var options = StartIndefinite ? UnityEditor.Progress.Options.Indefinite : UnityEditor.Progress.Options.None;
                m_ProgressId = UnityEditor.Progress.Start(OperationName, Description, options);
            }

            ProgressChanged?.Invoke(0.0f);
        }

        protected void Report()
        {
            if(Status != OperationStatus.InProgress)
                return;

            var progress = Progress;

            if (ShowInBackgroundTasks && StartIndefinite && progress > 0.0f &&
                (UnityEditor.Progress.GetOptions(m_ProgressId) & UnityEditor.Progress.Options.Indefinite) != 0)
            {
                UnityEditor.Progress.Remove(m_ProgressId);
                m_ProgressId = UnityEditor.Progress.Start(OperationName, Description,
                    IsSticky ? UnityEditor.Progress.Options.Sticky : UnityEditor.Progress.Options.None);
            }

            if (ShowInBackgroundTasks)
            {
                UnityEditor.Progress.Report(m_ProgressId, progress, Description);
            }

            ProgressChanged?.Invoke(progress);
        }

        public virtual void Finish(OperationStatus status)
        {
            Status = status;
            if (ShowInBackgroundTasks && UnityEditor.Progress.Exists(m_ProgressId))
            {
                UnityEditor.Progress.Finish(m_ProgressId, FromOperationStatus(status));
            }

            Finished?.Invoke(status);
        }

        public void Remove()
        {
            if (ShowInBackgroundTasks && UnityEditor.Progress.Exists(m_ProgressId))
            {
                UnityEditor.Progress.Remove(m_ProgressId);
            }
        }

        public void Pause()
        {
            Status = OperationStatus.Paused;
            ProgressChanged?.Invoke(0.0f);
        }

        public void Resume()
        {
            if (Status == OperationStatus.Paused)
            {
                Status = OperationStatus.InProgress;
                Report();
            }
        }

        static Progress.Status FromOperationStatus(OperationStatus status)
        {
            return status switch
            {
                OperationStatus.InProgress => UnityEditor.Progress.Status.Running,
                OperationStatus.Success => UnityEditor.Progress.Status.Succeeded,
                OperationStatus.Cancelled => UnityEditor.Progress.Status.Canceled,
                OperationStatus.Error => UnityEditor.Progress.Status.Failed,
                OperationStatus.None => UnityEditor.Progress.Status.Succeeded,
                OperationStatus.Paused => UnityEditor.Progress.Status.Paused,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}
