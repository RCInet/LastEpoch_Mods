using System;
using System.Net.Http;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// The EmptyNetworkTrace is meant to be a low overhead placeholder for the
    /// network trace mechanism. It implements the <seealso cref="INetworkTrace"/>
    /// interface in a way that a single instance can be recycled for all traces,
    /// thus eliminating garbage, and where all function calls are empty and return
    /// immediately.
    /// </summary>
    class EmptyNetworkTrace : INetworkTrace
    {
        public IProgress<HttpProgress> CreateProgressTracer(IProgress<HttpProgress> progress)
        {
            return progress;
        }

        public void Dispose()
        {
            //
            //  Intentionally left blank
            //
        }

        public void SetRequestData(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            //
            //  Intentionally left blank
            //
        }

        public void SetResponseData(HttpResponseMessage response)
        {
            //
            //  Intentionally left blank
            //
        }
    }
}
