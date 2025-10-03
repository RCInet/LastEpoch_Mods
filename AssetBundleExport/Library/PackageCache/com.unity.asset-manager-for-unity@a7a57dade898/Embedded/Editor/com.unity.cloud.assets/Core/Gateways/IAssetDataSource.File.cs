using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        Task<Uri> CreateFileAsync(DatasetDescriptor datasetDescriptor, IFileCreateData fileCreation, CancellationToken cancellationToken);

        IAsyncEnumerable<IFileData> ListFilesAsync(DatasetDescriptor datasetDescriptor, Range range, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        Task<IFileData> GetFileAsync(FileDescriptor fileDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        Task UpdateFileAsync(FileDescriptor fileDescriptor, IFileBaseData fileUpdate, CancellationToken cancellationToken);

        Task<Uri> GetFileDownloadUrlAsync(FileDescriptor fileDescriptor, int? maxDimension, CancellationToken cancellationToken);

        Task<Uri> GetFileUploadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken cancellationToken);

        Task FinalizeFileUploadAsync(FileDescriptor fileDescriptor, bool disableAutomaticTransformations, CancellationToken cancellationToken);

        Task<FileTag[]> GenerateFileTagsAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken);

        Task RemoveFileMetadataAsync(FileDescriptor fileDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
