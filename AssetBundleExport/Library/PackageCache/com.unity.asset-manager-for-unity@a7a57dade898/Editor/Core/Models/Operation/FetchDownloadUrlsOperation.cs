using System;

namespace Unity.AssetManager.Core.Editor
{
    struct FetchDownloadUrlsProgress
    {
        public FetchDownloadUrlsProgress(string description, float progress)
        {
            Description = description;
            Progress = progress;
        }

        public float Progress { get; } // between 0.0 and 1.0
        public string Description { get; }
    }

    class FetchDownloadUrlsOperation : BaseOperation, IProgress<FetchDownloadUrlsProgress>
    {
        public override string OperationName => "Fetching download URLs";
        public override string Description => m_Description;
        public override float Progress => m_Progress;

        string m_Description;
        float m_Progress;

        public void SetDescription(string description)
        {
            m_Description = description;
        }

        public void SetProgress(float progress)
        {
            m_Progress = progress;

            Report();
        }

        public void Report(FetchDownloadUrlsProgress value)
        {
            SetDescription(value.Description);
            SetProgress(value.Progress);
        }
    }
}
