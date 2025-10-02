using System;
using System.Net.Http;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// The network trace implements the IDisposable interface. It is meant to be disposed
    /// once the network call is completed, at which point the trace will be logged into
    /// the target format.
    /// </summary>
    interface INetworkTrace : IDisposable
    {
        IProgress<HttpProgress> CreateProgressTracer(IProgress<HttpProgress> progress);
        void SetRequestData(HttpRequestMessage request, HttpCompletionOption completionOption);
        void SetResponseData(HttpResponseMessage response);
    }
}
