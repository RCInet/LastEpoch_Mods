using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Buffers;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    sealed class ReaderStream : Stream
    {
        readonly QueueStream m_Stream;

        internal ReaderStream(QueueStream stream) => m_Stream = stream;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                m_Stream.DisposeReader();

            base.Dispose(disposing);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException(); // since CanSeek = false

        public override long Position
        {
            get => throw new NotSupportedException(); // since CanSeek = false
            set => throw new NotSupportedException(); // since CanSeek = false
        }

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                var source = m_Stream.Stream;
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(new Memory<byte>(buffer), cancellationToken).ConfigureAwaitFalse()) != 0)
                {
                    await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken).ConfigureAwaitFalse();
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public override int Read(byte[] buffer, int offset, int count) => m_Stream.Stream.Read(buffer, offset, count);
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => m_Stream.Stream.ReadAsync(buffer, offset, count, cancellationToken);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => m_Stream.Stream.ReadAsync(buffer, cancellationToken);

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException(); // since CanWrite = false
        public override void Flush() { /* do nothing */ }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(); // since CanSeek = false
        public override void SetLength(long value) => throw new NotSupportedException(); // since CanSeek = false
    }
}
