using System;
using System.IO;
using System.Threading;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// The QueueStream class represent a queue of data, where one producer writes to the "Writer" Stream and
    /// one consumer reads using the "Reader" Stream. Concurrent read and write are supported.
    ///
    /// The api of QueueStream is kept simply by design. Aside from construction, it simply gives access to a "Reader" Stream
    /// and a "Writer" Stream. Both "Reader" and "Writer" Stream object needs to be Disposed.
    /// </summary>
    class QueueStream
    {
        readonly ReaderStream m_Reader;
        readonly WriterStream m_Writer;

        AutoResetEvent m_AutoResetEvent = new (false);
        readonly object m_Lock = new ();

        bool m_ReaderDisposed;
        bool m_WriterDisposed;
        bool m_Disposed;

        public QueueStream()
        {
            Stream = new SingleReaderSingleWriterNativeStream();

            m_Reader = new ReaderStream(this);
            m_Writer = new WriterStream(this);
        }

        public Stream Reader => m_Reader;
        public Stream Writer => m_Writer;

        public SingleReaderSingleWriterNativeStream Stream { get; private set; }

        public void DisposeReader()
        {
            if (m_ReaderDisposed)
                return;

            m_ReaderDisposed = true;
            TryDispose();
        }

        void DisposeWriter()
        {
            if (m_WriterDisposed)
                return;

            m_WriterDisposed = true;
            Stream.SignalEndOfStream();
            TryDispose();
        }

        void TryDispose()
        {
            // Use lock to force a full fence so reader and writer don't need to be volatile
            lock (m_Lock)
            {
                if (m_ReaderDisposed && m_WriterDisposed)
                {
                    if (!m_Disposed)
                    {
                        Stream.Dispose();
                        Stream = null;

                        m_AutoResetEvent.Set();
                        m_AutoResetEvent.Dispose();
                        m_AutoResetEvent = null;

                        m_Disposed = true;
                    }
                }
            }
        }

        class WriterStream : Stream
        {
            readonly QueueStream m_Stream;

            public WriterStream(QueueStream stream)
            {
                m_Stream = stream;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException(); // since CanSeek = false

            public override long Position
            {
                get => throw new NotSupportedException(); // since CanSeek = false
                set => throw new NotSupportedException(); // since CanSeek = false
            }

            public override void Write(byte[] buffer, int offset, int count) => m_Stream.Stream.Write(buffer, offset, count);
            public override void Write(ReadOnlySpan<byte> buffer) => m_Stream.Stream.Write(buffer);

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    m_Stream.DisposeWriter();

                base.Dispose(disposing);
            }

            public override void Flush() { /* do nothing */ }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException(); // since CanRead = false
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(); // since CanSeek = false
            public override void SetLength(long value) => throw new NotSupportedException(); // since CanSeek = false
        }
    }
}
