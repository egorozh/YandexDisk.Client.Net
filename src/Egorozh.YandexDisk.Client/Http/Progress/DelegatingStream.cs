﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Egorozh.YandexDisk.Client.Http.Progress;

internal abstract class DelegatingStream : Stream
{
    private readonly Stream _innerStream;

    protected DelegatingStream(Stream innerStream)
    {
        ArgumentNullException.ThrowIfNull(innerStream);
        
        _innerStream = innerStream;
    }

    protected Stream InnerStream => _innerStream;

    public override bool CanRead => _innerStream.CanRead;

    public override bool CanSeek => _innerStream.CanSeek;

    public override bool CanWrite => _innerStream.CanWrite;

    public override long Length => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public override int ReadTimeout
    {
        get => _innerStream.ReadTimeout;
        set => _innerStream.ReadTimeout = value;
    }

    public override bool CanTimeout => _innerStream.CanTimeout;

    public override int WriteTimeout
    {
        get => _innerStream.WriteTimeout;
        set => _innerStream.WriteTimeout = value;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerStream.Dispose();
        }
        base.Dispose(disposing);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

#if !NETSTANDARD1_3 // BeginX and EndX not supported on Streams in netstandard1.3
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        return _innerStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return _innerStream.EndRead(asyncResult);
    }
#endif

    public override int ReadByte()
    {
        return _innerStream.ReadByte();
    }

    public override void Flush()
    {
        _innerStream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _innerStream.FlushAsync(cancellationToken);
    }

    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

#if !NETSTANDARD1_3 // BeginX and EndX not supported on Streams in netstandard1.3
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        return _innerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        _innerStream.EndWrite(asyncResult);
    }
#endif

    public override void WriteByte(byte value)
    {
        _innerStream.WriteByte(value);
    }
}