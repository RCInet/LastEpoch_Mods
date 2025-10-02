using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Editor
{
    internal class CloudProjectSettingsUnityEditorAccessTokenProvider : IUnityEditorAccessTokenProvider
    {
        string m_AccessToken;
        readonly SynchronizationContext m_SynchronizationContext;

        internal CloudProjectSettingsUnityEditorAccessTokenProvider()
        {
            m_AccessToken = CloudProjectSettings.accessToken;
            m_SynchronizationContext = SynchronizationContext.Current;
        }

        public Task<string> GetAccessTokenAsync()
        {
            PostAccessTokenOnMainThread();
            return Task.FromResult(m_AccessToken);
        }

        // CloudProjectSettings can only be reach from main thread
        void PostAccessTokenOnMainThread()
        {
            m_SynchronizationContext.Post( _ =>
            {
                m_AccessToken = CloudProjectSettings.accessToken;
            }, null);
        }

        public string GetAccessToken()
        {
            PostAccessTokenOnMainThread();
            return m_AccessToken;
        }
    }
}
