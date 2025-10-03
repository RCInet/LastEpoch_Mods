namespace Unity.AssetManager.Core.Editor
{
    enum CacheValidationResultError
    {
        None,
        PathTooLong,
        InvalidPath,
        CannotWriteToDirectory,
        DirectoryNotFound
    }

    class CacheLocationValidationResult
    {
        public bool Success { get; set; }
        public CacheValidationResultError ErrorType { get; set; }
    }
}
