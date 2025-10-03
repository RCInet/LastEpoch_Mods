using System;

namespace Unity.AssetManager.Core.Editor
{
    enum RecommendedAction
    {
        OpenServicesSettingButton,
        OpenAssetManagerDashboardLink,
        EnableProject,
        OpenAssetManagerDocumentationPage,
        Retry,
        None
    }

    [Serializable]
    class Message
    {
        Guid m_MessageId;
        string m_Content;
        RecommendedAction m_RecommendedAction;

        public Guid MessageId => m_MessageId;
        public string Content => m_Content;
        public RecommendedAction RecommendedAction => m_RecommendedAction;

        public Message(string content, RecommendedAction recommendedAction = RecommendedAction.None)
        {
            m_MessageId = Guid.NewGuid();

            m_Content = content;
            m_RecommendedAction = recommendedAction;
        }
    }
}
