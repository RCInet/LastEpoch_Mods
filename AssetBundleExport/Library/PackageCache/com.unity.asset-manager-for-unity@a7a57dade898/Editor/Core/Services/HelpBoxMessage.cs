using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class HelpBoxMessage : Message
    {
        HelpBoxMessageType m_MessageType;

        public HelpBoxMessageType MessageType => m_MessageType;

        public HelpBoxMessage(string content, RecommendedAction recommendedAction = RecommendedAction.None,
            HelpBoxMessageType messageType = HelpBoxMessageType.None)
            : base(content, recommendedAction)
        {
            m_MessageType = messageType;
        }
    }
}
