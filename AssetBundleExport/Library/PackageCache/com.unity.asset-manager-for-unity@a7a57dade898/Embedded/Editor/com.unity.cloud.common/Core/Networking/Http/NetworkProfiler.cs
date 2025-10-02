using System;

namespace Unity.Cloud.CommonEmbedded
{
    static class NetworkProfiler
    {
        static readonly INetworkTrace k_Default = new EmptyNetworkTrace();

        static Func<INetworkTrace> s_NetworkTraceProvider;

        public static void RegisterNetworkTraceProvider(Func<INetworkTrace> provider)
        {
            s_NetworkTraceProvider = provider;
        }

        public static INetworkTrace Trace()
        {
            if (s_NetworkTraceProvider == null)
            {
                return k_Default;
            }
            else
            {
                return s_NetworkTraceProvider.Invoke();
            }
        }
    }
}
