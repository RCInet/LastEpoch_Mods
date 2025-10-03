using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.Core.Editor
{
    interface IMessageManager : IService
    {
        public HelpBoxMessage HelpBoxMessage { get; }
        public Message GridViewMessage { get; }

        void SetHelpBoxMessage(HelpBoxMessage helpBoxMessage);
        void ClearHelpBoxMessage();

        void SetGridViewMessage(Message message);
        void ClearGridViewMessage();

        void ClearAllMessages();

        event Action<HelpBoxMessage> HelpBoxMessageSet;
        event Action HelpBoxMessageCleared;

        event Action<Message> GridViewMessageSet;
        event Action GridViewMessageCleared;
    }

    [Serializable]
    class MessageManager : BaseService<IMessageManager>, IMessageManager
    {
        HelpBoxMessage m_HelpBoxMessage;
        Message m_GridViewMessage;

        public HelpBoxMessage HelpBoxMessage => m_HelpBoxMessage;
        public Message GridViewMessage => m_GridViewMessage;

        public event Action<HelpBoxMessage> HelpBoxMessageSet;
        public event Action HelpBoxMessageCleared;

        public event Action<Message> GridViewMessageSet;
        public event Action GridViewMessageCleared;

        public void SetHelpBoxMessage(HelpBoxMessage helpBoxMessage)
        {
            m_HelpBoxMessage = helpBoxMessage;

            HelpBoxMessageSet?.Invoke(helpBoxMessage);
        }

        public void ClearHelpBoxMessage()
        {
            m_HelpBoxMessage = null;

            HelpBoxMessageCleared?.Invoke();
        }

        public void SetGridViewMessage(Message message)
        {
            m_GridViewMessage = message;

            GridViewMessageSet?.Invoke(message);
        }

        public void ClearGridViewMessage()
        {
            m_GridViewMessage = null;

            GridViewMessageCleared?.Invoke();
        }

        public void ClearAllMessages()
        {
            ClearHelpBoxMessage();
            ClearGridViewMessage();
        }
    }
}
