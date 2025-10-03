using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// Stream that writes directly to a Unity <see cref="NativeArray{T}"/>.
    /// </summary>
    /// <remarks>
    /// Supports a single producer and a single consumer. The producer can
    /// write while the consumer reads concurrently. Two producers cannot
    /// write at the same time, nor consumers can read at the same time.
    /// </remarks>
    class SingleReaderSingleWriterNativeStream : Stream
    {
        const int k_ChunkSize = 128 * 1024;

        readonly object m_Lock = new();
        readonly List<NativeArray<byte>> m_Buffers = new();

        long m_WriterIndex;
        long m_ReaderIndex;
        bool m_EndOfStream;
        bool m_Disposed;

        AsyncAutoResetEvent m_Sync = new();

        ~SingleReaderSingleWriterNativeStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            m_Disposed = true;

            if (disposing)
            {
                m_Sync.Dispose();
                m_Sync = null;
            }

            foreach (var b in m_Buffers)
                b.Dispose();

            base.Dispose(disposing);
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public void SignalEndOfStream()
        {
            m_EndOfStream = true;
            m_Sync.Set();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_Disposed)
                throw new ObjectDisposedException(nameof(SingleReaderSingleWriterNativeStream));

            if (!m_EndOfStream && !TaskExtensions.MultithreadingEnabled)
                throw new InvalidOperationException($"Cannot use {nameof(Read)} method on incomplete streams on platforms that don't support multithreading since it's going to freeze the app.");

            return ReadAsync(buffer, offset, count, CancellationToken.None).Result;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (m_Disposed)
                throw new ObjectDisposedException(nameof(SingleReaderSingleWriterNativeStream));

            while (true)
            {
                if (m_Disposed)
                    return 0;

                if (m_ReaderIndex == m_WriterIndex && !m_EndOfStream)
                    await m_Sync.WaitAsync(cancellationToken).ConfigureAwaitFalse();
                else
                    break;
            }

            return ReadToBuffer(buffer);
        }

        int ReadToBuffer(Memory<byte> buffer)
        {
            var readerIndex = m_ReaderIndex;
            var writerIndex = m_WriterIndex;
            var read = 0;
            var remainingToRead = Math.Min(writerIndex - readerIndex, buffer.Length);

            while (remainingToRead > 0)
            {
                var readIndex = (int)(readerIndex / k_ChunkSize);

                var offset = (int)(readerIndex % k_ChunkSize);
                var nativeSpace = k_ChunkSize - offset;
                var toRead = (int)Math.Min(remainingToRead, nativeSpace);

                m_Buffers[readIndex].AsSpan().Slice(offset, toRead).CopyTo(buffer.Span[read..]);

                readerIndex += toRead;
                read += toRead;
                remainingToRead -= toRead;

                if (toRead == nativeSpace)
                {
                    lock (m_Lock)
                    {
                        // Transfer the buffer at the end of the list to minimize managed<->native calls,
                        // don't RemoveAt(0) from the list since it may get costly for bigger streams.
                        // Having a real resizable circular buffer would be more optimal here.
                        m_Buffers.Add(m_Buffers[readIndex]);

                        // Needs to remove the reference else NativeArray throws when Dispose is called twice.
                        m_Buffers[readIndex] = new NativeArray<byte>();
                    }
                }
            }

            m_ReaderIndex = readerIndex;

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => Write(buffer.AsSpan(offset, count));
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (m_Disposed)
                throw new ObjectDisposedException(nameof(SingleReaderSingleWriterNativeStream));

            var writerIndex = m_WriterIndex;
            var written = 0;
            var remainingToWrite = buffer.Length;

            while (remainingToWrite > 0)
            {
                var writeIndex = (int)(writerIndex / k_ChunkSize);

                lock (m_Lock)
                {
                    if (writeIndex == m_Buffers.Count)
                        m_Buffers.Add(new NativeArray<byte>(k_ChunkSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory));
                }

                var offset = (int)(writerIndex % k_ChunkSize);
                var nativeSpace = k_ChunkSize - offset;
                var toWrite = Math.Min(remainingToWrite, nativeSpace);

                buffer.Slice(written, toWrite).CopyTo(m_Buffers[writeIndex].AsSpan()[offset..]);

                writerIndex += toWrite;
                written += toWrite;
                remainingToWrite -= toWrite;
            }

            m_WriterIndex = writerIndex;
            m_Sync.Set();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Write(buffer.Span);
            return default;
        }

        sealed class AsyncAutoResetEvent : IDisposable
        {
            static readonly Task k_Completed = Task.FromResult(true);

            readonly Queue<(TaskCompletionSource<bool> Source, CancellationTokenRegistration Ctr)> m_PendingSources = new();

            bool m_Signaled;

            public void Dispose()
            {
                foreach (var (s, c) in m_PendingSources)
                {
                    s.TrySetCanceled();
                    c.Dispose();
                }
            }

            public Task WaitAsync(CancellationToken token = default)
            {
                lock (m_PendingSources)
                {
                    token.ThrowIfCancellationRequested();

                    if (m_Signaled)
                    {
                        m_Signaled = false;
                        return k_Completed;
                    }

                    var tcs = new TaskCompletionSource<bool>();
                    var ctr = token.Register(() => tcs.SetCanceled());
                    m_PendingSources.Enqueue((tcs, ctr));
                    return tcs.Task;
                }
            }

            public void Set()
            {
                (TaskCompletionSource<bool> Source, CancellationTokenRegistration Ctr) toRelease = default;

                lock (m_PendingSources)
                {
                    if (m_PendingSources.Count > 0)
                        toRelease = m_PendingSources.Dequeue();
                    else if (!m_Signaled)
                        m_Signaled = true;
                }

                toRelease.Ctr.Dispose();
                toRelease.Source?.SetResult(true);
            }
        }
    }
}
