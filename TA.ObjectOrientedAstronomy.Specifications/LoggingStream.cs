// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: LoggingStream.cs  Last modified: 2020-08-20@18:10 by Tim Long

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UtilityClasses;

public class LoggingStream : MemoryStream
    {
    internal ByteArrayBuilder outputLog = new ByteArrayBuilder();

    /// <summary>
    /// Gets the exact ASCII-encoded bytes written to the output stream.
    /// </summary>
    public byte[] OutputBytes => outputLog.ToArray();

    public int OutputByteCount => outputLog.Length;

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
        outputLog.Append(buffer);
        return base.WriteAsync(buffer, offset, count, cancellationToken);
        }
    }