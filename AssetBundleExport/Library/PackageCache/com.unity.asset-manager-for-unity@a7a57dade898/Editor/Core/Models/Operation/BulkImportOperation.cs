using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.AssetManager.Core.Editor
{
    class BulkImportOperation : BaseOperation
    {
        public override string OperationName => "Importing all selected assets";
        public override string Description { get; }
        public override float Progress
        {
            get
            {
                var totalProgress = 0f;
                foreach (var importOperation in m_ImportOperations)
                {
                    totalProgress += importOperation.Progress;
                }

                return m_ImportOperations.Count > 0 ? totalProgress / m_ImportOperations.Count : 0f;
            }
        }
        public override bool StartIndefinite => true;
        public override bool IsSticky => true;
        public override bool ShowInBackgroundTasks => true;

        List<ImportOperation> m_ImportOperations;
        ImportTrigger m_ImportTrigger;

        public BulkImportOperation(List<ImportOperation> importOperations, ImportTrigger importTrigger)
        {
            m_ImportOperations = importOperations;
            m_ImportTrigger = importTrigger;

            foreach (var importOperation in m_ImportOperations)
            {
                importOperation.Finished += OnImportCompleted;
                importOperation.ProgressChanged += OnImportProgressChanged;
            }
        }

        void OnImportCompleted(OperationStatus status)
        {
            if (m_ImportOperations.TrueForAll(x => x.Status is OperationStatus.Success
                or OperationStatus.Error or OperationStatus.Cancelled))
            {
                OperationStatus finalStatus;
                if(m_ImportOperations.Exists(x => x.Status == OperationStatus.Error))
                {
                    finalStatus = OperationStatus.Error;
                }
                else if(m_ImportOperations.Exists(x => x.Status == OperationStatus.Cancelled))
                {
                    finalStatus = OperationStatus.Cancelled;
                }
                else
                {
                    finalStatus = OperationStatus.Success;
                }

                SendImportEndAnalytic(finalStatus);
                Finish(finalStatus);
            }
        }

        void OnImportProgressChanged(float progress)
        {
            Report();
        }

        void SendImportEndAnalytic(OperationStatus finalStatus)
        {
            string errorMessage = string.Empty;
            ImportEndStatus status;
            switch (finalStatus)
            {
                case OperationStatus.Success:
                    status = ImportEndStatus.Ok;
                    break;

                case OperationStatus.Cancelled:
                    status = ImportEndStatus.Cancelled;
                    break;

                case OperationStatus.Error:
                    status = ImportEndStatus.DownloadError;
                    foreach (var importOperation in m_ImportOperations)
                    {
                        if (importOperation.Status == OperationStatus.Error)
                        {
                            foreach (var downloadRequest in importOperation.DownloadRequests)
                            {
                                if (!string.IsNullOrEmpty(downloadRequest.Error))
                                {
                                    errorMessage = downloadRequest.Error;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;

                default:
                    status = ImportEndStatus.GenericError;
                    break;
            }

            AnalyticsSender.SendEvent(new ImportEndEvent(m_ImportTrigger,
                status,
                m_ImportOperations.Select(io => io.Identifier.AssetId).ToList(),
                m_ImportOperations.Min(io => io.StartTime),
                DateTime.Now,
                errorMessage));
        }
    }
}
