// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsReader.cs  Last modified: 2016-09-30@02:10 by Tim Long

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     An implementation of <see cref="StreamReader" /> that is useful for reading NASA FITS files.
    /// </summary>
    [CLSCompliant(true)]
    public class FitsReader
        {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Stream sourceStream;
        private int blockIndex = Constants.FitsBlockLength + 1;
        private byte[] CurrentBlock = new byte[0];

        public FitsReader(Stream sourceStream)
            {
            this.sourceStream = sourceStream;
            }

        private bool BlockIsEmpty
            =>
            blockIndex >= Constants.FitsBlockLength || blockIndex < 0 ||
            CurrentBlock.Length != Constants.FitsBlockLength;

        internal int BlockBytesRemaining => BlockIsEmpty ? 0 : Constants.FitsBlockLength - blockIndex;

        /// <summary>
        ///     Reads one block from the source stream and verifies that a complete block was read.
        /// </summary>
        /// <returns>
        ///     An array of ASCII-encoded bytes which will always be exactly <see cref="Constants.FitsBlockLength" /> bytes.
        /// </returns>
        public async Task<byte[]> ReadBlock()
            {
            var buffer = new byte[Constants.FitsBlockLength];
            var bytesRead = await sourceStream.ReadAsync(buffer, 0, Constants.FitsBlockLength).ConfigureAwait(false);
            if (bytesRead != Constants.FitsBlockLength)
                throw new IOException($"Unable to read a complete FITS block of {Constants.FitsBlockLength} bytes");
            return buffer;
            }

        /// <summary>
        ///     Reads exactly the specified number of characters from the block buffer, refilling the buffer as
        ///     necessary. The ASCII-encoded source characters are decoded to normal UTF-8 Unicode characters.
        /// </summary>
        /// <param name="charactersRequested">
        ///     The total number of characters to be read from one or more blocks of data.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="Task{String}" /> which upon completion will contain the requested number of characters.
        /// </returns>
        private async Task<string> BlockReadExactly(int charactersRequested)
            {
            var charactersOutstanding = charactersRequested;
            var builder = new StringBuilder();
            while (builder.Length < charactersRequested)
                {
                if (BlockIsEmpty) await FillCurrentBlock().ConfigureAwait(false);
                if (BlockBytesRemaining >= charactersOutstanding)
                    {
                    var utf8 = DecodeAsciiFromCurrentBlock(charactersOutstanding);
                    blockIndex += charactersOutstanding;
                    builder.Append(utf8);
                    return builder.ToString();
                    }
                builder.Append(DecodeAsciiFromCurrentBlock(BlockBytesRemaining));
                charactersOutstanding -= BlockBytesRemaining;
                blockIndex += BlockBytesRemaining;
                }
            return builder.ToString();
            }

        private char[] DecodeAsciiFromCurrentBlock(int characterCount)
            {
            if (characterCount > BlockBytesRemaining)
                throw new ArgumentOutOfRangeException("characterCount", characterCount,
                    $"{characterCount} characters were requested but there are only {BlockBytesRemaining} in the block buffer");
            var decodedCharacters = Encoding.ASCII.GetChars(CurrentBlock, blockIndex, characterCount);
            return decodedCharacters;
            }

        private async Task FillCurrentBlock()
            {
            CurrentBlock = await ReadBlock().ConfigureAwait(false);
            blockIndex = 0;
            }

        /// <summary>
        ///     Asynchronously reads one record.
        /// </summary>
        /// <returns>A <see cref="Task{String}" /> that will contain the record upon completion.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a complete record could not be read.</exception>
        /// <remarks>
        ///     FITS records are based on punched cards and each record contains exactly 80 characters, encoded as ASCII
        ///     bytes. When reading a record, exactly 80 bytes are read from the underlying stream. The bytes are then
        ///     converted to a unicode string using UTF-8 encoding.
        /// </remarks>
        public async Task<FitsRecord> ReadRecord()
            {
            var record = await BlockReadExactly(Constants.FitsRecordLength).ConfigureAwait(false);
            Log.Debug($"Read record: [{record}]");
            if (record.Length != Constants.FitsRecordLength)
                {
                throw new InvalidOperationException(
                    $"Tried to read an {Constants.FitsRecordLength}-byte record but received {record.Length} bytes");
                }
            return FitsRecord.FromString(record);
            }

        public async Task<FitsHeaderRecord> ReadHeaderRecord()
            {
            var rawRecord = await ReadRecord().ConfigureAwait(false);
            return FitsHeaderRecord.FromString(rawRecord.Text);
            }

        /// <summary>
        ///     Reads the primary header.
        /// </summary>
        /// <returns>FitsHeader.</returns>
        public async Task<FitsHeader> ReadPrimaryHeader()
            {
            var result = new FitsHeader();
            var currentRecord = FitsHeaderRecord.Empty;
            do
                {
                try
                    {
                    currentRecord = await ReadHeaderRecord().ConfigureAwait(false);
                    result.AppendHeaderRecord(currentRecord);
                    }
                catch (InvalidHeaderRecordException ex)
                    {
                    Log.Warn(ex, $"ignoring invalid header record: [{ex.Record}]");
                    }
                } while (currentRecord.Keyword != Constants.EndKeyword);
            return result;
            }
        }
    }