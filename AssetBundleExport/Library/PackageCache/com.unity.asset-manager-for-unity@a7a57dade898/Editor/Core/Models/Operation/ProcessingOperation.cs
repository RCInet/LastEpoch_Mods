using System.Threading.Tasks;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    class ProcessingOperation : BaseOperation
    {
        public override float Progress => 0.0f;
        public override string OperationName => "Processing";
        public override string Description => "Checking for dependencies, updates and conflicts";
        public override bool StartIndefinite => true;
        public override bool ShowInBackgroundTasks => true;

        public override void Start()
        {
            base.Start();

            Task.Run(async () =>
            {
                while (Status == OperationStatus.InProgress)
                {
                    await Task.Delay(4000); // 4 seconds because a background task became in unresponsive state after
                                            // 5 seconds without reporting progress

                    // Check status again in case the operation was finished while waiting
                    if (Status == OperationStatus.InProgress)
                    {
                        Report();
                    }
                }
            });
        }
    }
}
