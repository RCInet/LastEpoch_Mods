using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    class MemoryStreamDownloadHandler : DownloadHandlerScript
    {
        public int ContentLength => m_BytesReceived > m_ContentLength ? m_BytesReceived : m_ContentLength;

        public event Action HeadersReceived;

        Stream m_Writer;

        public bool DataReceived { get; private set; }

        int m_ContentLength;
        int m_BytesReceived;
        bool m_HeadersReceivedInvoked;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">Takes ownership of the writer. This class will call Dispose on it</param>
        /// <param name="bufferSize"></param>
        public MemoryStreamDownloadHandler(Stream writer, int bufferSize = 4096) : base(new byte[bufferSize])
        {
            m_ContentLength = -1;
            m_BytesReceived = 0;
            m_HeadersReceivedInvoked = false;
            DataReceived = false;
            m_Writer = writer;
        }

        protected override float GetProgress()
        {
            return ContentLength <= 0 ? 0 : Mathf.Clamp01((float)m_BytesReceived / (float)ContentLength);
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            m_ContentLength = (int)contentLength;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length == 0)
                return false;

            TryInvokeHeadersReceived();

            m_BytesReceived += dataLength;
            m_Writer.Write(data, 0, dataLength);

            DataReceived = true;
            return true;
        }

        protected override void CompleteContent()
        {
            TryInvokeHeadersReceived();

            CloseStream();
        }

        /// <summary>
        /// Act as the real/effective Dispose method. To be used instead of Dispose.
        ///
        /// Sadly, the parent class DownloadHandler does not properly
        /// implement the Dispose pattern and does not distinguish between a user call to Dispose from the Finalizer
        /// running. The result is that from a overriden Dispose, we cannot know if it's safe to Dispose other internal
        /// objects.
        /// </summary>
        public void EffectiveDispose()
        {
            CloseStream();

            Dispose();
#pragma warning disable S3971 // "GC.SuppressFinalize" should not be called
            GC.SuppressFinalize(this);
#pragma warning restore S3971
        }

        /// <summary>
        /// Used in the event of an error that prevents the download handler from calling complete content.
        /// </summary>
        public void ForceCompleteContent()
        {
            CompleteContent();
        }

        void TryInvokeHeadersReceived()
        {
            if (!m_HeadersReceivedInvoked)
            {
                HeadersReceived?.Invoke();
                m_HeadersReceivedInvoked = true;
            }
        }

        void CloseStream()
        {
            if (m_Writer != null)
            {
                m_Writer.Dispose();
                m_Writer = null;
            }
        }
    }
}
