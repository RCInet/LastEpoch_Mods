using System;

namespace Unity.AssetManager.Core.Editor
{
    struct LoadDependenciesProgress
    {
        public LoadDependenciesProgress(string description, float progress)
        {
            Description = description;
            Progress = progress;
        }

        public float Progress { get; } // between 0.0 and 1.0
        public string Description { get; }
    }

    class LoadDependenciesOperation : BaseOperation, IProgress<LoadDependenciesProgress>
    {
        public override string OperationName => "Loading dependencies";
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

        public void Report(LoadDependenciesProgress value)
        {
            SetDescription(value.Description);
            SetProgress(value.Progress);
        }
    }
}
