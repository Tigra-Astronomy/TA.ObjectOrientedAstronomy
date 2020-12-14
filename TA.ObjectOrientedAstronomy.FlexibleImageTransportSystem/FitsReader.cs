// This file is part of the TA.ObjectOrientedAstronomy project
//
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
//
// File: FitsReader.cs  Last modified: 2016-10-13@23:19 by Tim Long

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using TA.Utils.Core;
using TA.Utils.Core.Diagnostics;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     An implementation of <see cref="StreamReader" /> that is useful for reading NASA FITS files.
    /// </summary>
    public class FitsReader
        {
        private readonly Stream sourceStream;
        private readonly ILog log;
        private int blockIndex = FitsFormat.FitsBlockLength + 1;
        private byte[] currentBlock = new byte[0];

        public FitsReader(Stream sourceStream, ILog log = null)
            {
            this.sourceStream = sourceStream;
            this.log = log ?? new DegenerateLoggerService();
            }

        private bool BlockIsEmpty
            =>
            blockIndex >= FitsFormat.FitsBlockLength || blockIndex < 0 ||
            currentBlock.Length != FitsFormat.FitsBlockLength;

        internal int BlockBytesRemaining => BlockIsEmpty ? 0 : FitsFormat.FitsBlockLength - blockIndex;

        /// <summary>
        ///     Reads one block from the source stream and verifies that a complete block was read.
        /// </summary>
        /// <returns>
        ///     An array of ASCII-encoded bytes which will always be exactly <see cref="FitsFormat.FitsBlockLength" /> bytes.
        /// </returns>
        public async Task<byte[]> ReadBlock()
            {
            var buffer = new byte[FitsFormat.FitsBlockLength];
            var bytesRead = await sourceStream.ReadAsync(buffer, 0, FitsFormat.FitsBlockLength).ConfigureAwait(false);
            if (bytesRead != FitsFormat.FitsBlockLength)
                throw new FitsIncompleteBlockException();
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
            var decodedCharacters = Encoding.ASCII.GetChars(currentBlock, blockIndex, characterCount);
            return decodedCharacters;
            }

        /// <summary>
        ///     Moves to block boundary. If the read pointer is already at a block boundary, nothing happens.
        /// </summary>
        private async Task MoveToBlockBoundary()
            {
            if (!BlockIsEmpty)
                await FillCurrentBlock().ConfigureAwait(false);
            }

        private async Task<byte[]> BlockReadBytes(int byteCount)
            {
            var bytesOutstanding = byteCount;
            var bufferIndex = 0;
            var buffer = new byte[byteCount];
            while (bytesOutstanding > 0)
                {
                if (BlockIsEmpty)
                    await FillCurrentBlock().ContinueOnAnyThread();
                if (BlockBytesRemaining >= bytesOutstanding)
                    {
                    // There are sufficient bytes in the block to completely satisfy the request
                    Array.ConstrainedCopy(currentBlock, blockIndex, buffer, bufferIndex, bytesOutstanding);
                    blockIndex += bytesOutstanding;
                    bufferIndex += bytesOutstanding;
                    bytesOutstanding = 0;
                    return buffer;
                    }
                // There are insufficient bytes in the block, so empty the block into the buffer.
                Array.ConstrainedCopy(currentBlock, blockIndex, buffer, bufferIndex, BlockBytesRemaining);
                bufferIndex += BlockBytesRemaining;
                bytesOutstanding -= BlockBytesRemaining;
                blockIndex += BlockBytesRemaining;
                }
            return buffer;
            }

        private async Task FillCurrentBlock()
            {
            currentBlock = await ReadBlock().ConfigureAwait(false);
            blockIndex = 0;
            }

        /// <summary>
        ///     Asynchronously reads one record.
        /// </summary>
        /// <returns>A <see cref="Task{FitsRecord}" /> that will contain the record upon completion.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a complete record could not be read.</exception>
        /// <remarks>
        ///     FITS records are based on punched cards and each record contains exactly 80 characters, encoded as ASCII
        ///     bytes. When reading a record, exactly 80 bytes are read from the underlying stream. The bytes are then
        ///     converted to a unicode string using UTF-8 encoding.
        /// </remarks>
        public async Task<FitsRecord> ReadRecord()
            {
            var record = await BlockReadExactly(FitsFormat.FitsRecordLength).ConfigureAwait(false);
            log.Debug().Message("Read record: [{record}]", record).Write();
            if (record.Length != FitsFormat.FitsRecordLength)
                {
                throw new InvalidOperationException(
                    $"Tried to read an {FitsFormat.FitsRecordLength}-byte record but received {record.Length} bytes");
                }
            return FitsRecord.FromString(record);
            }

        public async Task<FitsHeaderRecord> ReadHeaderRecord()
            {
            var rawRecord = await ReadRecord().ConfigureAwait(false);
            return FitsHeaderRecord.FromRecordText(rawRecord.Text, log);
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
                    log.Warn().Message("ignoring invalid header record: [{record}]", ex.Record).Exception(ex);
                    }
                } while (currentRecord.Keyword != FitsFormat.EndKeyword);
            log.Debug().Message("Read FITS header").Property("header",result).Write();
            return result;
            }

        public async Task<FitsHeaderDataUnit> ReadPrimaryHeaderDataUnit()
            {
            var hdu = new FitsHeaderDataUnit();
            hdu.Header = await ReadPrimaryHeader().ConfigureAwait(false);
            hdu.MandatoryKeywords = hdu.Header.HeaderRecords.BindProperties<FitsPrimaryHduMandatoryKeywords>();
            await MoveToBlockBoundary().ConfigureAwait(false);
            if (hdu.MandatoryKeywords.NumberOfAxes == 0)
                {
                hdu.DataType = FitsDataType.None;
                hdu.DataArrayLengthBits = 0;
                hdu.RawData = new byte[0];
                return hdu;
                }
            var dataArrayBits = ComputeDataArrayBitLength(hdu.MandatoryKeywords);
            var dataArrayBytes = dataArrayBits / 8;
            hdu.RawData = await BlockReadBytes(dataArrayBytes).ContinueOnAnyThread();
            hdu.DataArrayLengthBits = dataArrayBits;
            hdu.DataType = FitsDataType.Image;
            return hdu;
            }

        /// <summary>
        ///     Computes the length of the data array, in bits, using the formula:
        ///     <c>Nbits = |BITPIX| × (NAXIS1 × NAXIS2 × · · · × NAXISm)</c>
        /// </summary>
        /// <param name="headerValues">The mandatory FITS header values which specify the type and size of the data array.</param>
        /// <returns>The number of bits contained in the data array.</returns>
        private static int ComputeDataArrayBitLength(FitsMandatoryKeywords headerValues)
            {
            var dataArrayBits = Math.Abs(headerValues.BitsPerPixel);
            foreach (var axisLength in headerValues.LengthOfAxis)
                {
                dataArrayBits *= axisLength;
                }
            return dataArrayBits;
            }
        }
    }