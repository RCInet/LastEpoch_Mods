namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Contains status information on the progress of an IHttpClient operation.
    /// </summary>
    struct HttpProgress
    {
        /// <summary>
        /// Floating-point value between 0.0 and 1.0 indicating the progress of downloading body data from the server.
        /// </summary>
        public float? DownloadProgress { get; set; }

        /// <summary>
        /// Floating-point value between 0.0 and 1.0 indicating the progress of uploading body data to the server.
        /// </summary>
        public float? UploadProgress { get; set; }

        /// <summary>
        /// Initializes and returns an instance of <see cref="HttpProgress"/>.
        /// </summary>
        /// <param name="downloadProgress">The progress of downloading body data from the server.</param>
        /// <param name="uploadProgress">The progress of uploading body data to the server.</param>
        public HttpProgress(float? downloadProgress, float? uploadProgress)
        {
            DownloadProgress = downloadProgress;
            UploadProgress = uploadProgress;
        }
    }
}
