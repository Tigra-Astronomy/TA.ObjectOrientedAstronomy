// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsWriter.cs  Last modified: 2020-08-20@14:58 by Tim Long

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using static TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.FitsFormat;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public class FitsWriter
        {
        internal WriterState state = WriterState.PrimaryHeader;
        readonly Stream stream;
        internal StringBuilder currentBlock;
        internal FitsPrimaryHduMandatoryKeywords headerMandatoryValues = new FitsPrimaryHduMandatoryKeywords();
        internal FitsHeader headerRecords = new FitsHeader();
        private bool writingHeader;

        public FitsWriter(Stream stream)
            {
            this.stream = stream;
            currentBlock = CreateEmptyBlock();
            }

        /// <summary>
        /// Gets the number of characters remaining in the current FITS block.
        /// </summary>
        public int BlockSpaceRemaining => FitsBlockLength - currentBlock.Length;

        /// <summary>
        /// Writes the supplied <paramref name="record"/> to the current FITS block.
        /// If the current block becomes full, it will be flushed to the output stream.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task WriteRecord(FitsRecord record)
            {
            currentBlock.Append(record.Text);
            if (BlockSpaceRemaining <= 0)
                await WriteBlock();
            }

        internal async Task WriteBlock()
            {
            // Ordering of actions is important here to prevent race conditions.
            var blockToWrite = currentBlock.ToString();
            currentBlock = CreateEmptyBlock();
            var bytesToWrite = Encoding.ASCII.GetBytes(blockToWrite);
            var blockLength = bytesToWrite.Length;
            if (blockLength != FitsBlockLength)
                throw new FitsFormatException(
                    $"FITS blocks must be {FitsBlockLength} bytes but {blockLength} bytes were supplied. This is probably a bug, please report via GitHub.");
            await stream.WriteAsync(bytesToWrite, 0, blockLength);
            }

        private static StringBuilder CreateEmptyBlock()
            {
            return new StringBuilder(FitsBlockLength);
            }

        internal bool BlockIsEmpty => currentBlock.Length == 0;

        /// <summary>
        /// Flushes any pending data and closes the underlying output stream.
        /// </summary>
        public void Close()
            {
            if (!BlockIsEmpty)
                FlushBlock().Wait();
            stream.Dispose();
            }

        /// <summary>
        /// Flushes a non-empty block to the output stream, padding with fill characters as necessary.
        /// Empty blocks cannot be flushed and produce no output.
        /// </summary>
        /// <returns>A task that completes when the block is fully written to the output stream.</returns>
        internal async Task FlushBlock()
            {
            char padByte = WritingHeader ? PadCharacter : '\0';
            if (BlockIsEmpty) return;
            var bytesToPad = BlockSpaceRemaining;
            if (bytesToPad > 0)
                currentBlock.Append(padByte, bytesToPad);
            await WriteBlock();
            }

        /// <summary>
        /// Writes a header record into the current header block.
        /// </summary>
        /// <param name="record"></param>
        public async Task WriteHeaderRecord(FitsHeaderRecord record)
            {
            if (WritingData)
                throw new FitsFormatException($"Attempt to write a header record into a data block: {record}");
            headerRecords.AppendHeaderRecord(record); // Keep track of records we've already written.
            headerMandatoryValues =
                headerRecords.HeaderRecords.BindProperties<FitsPrimaryHduMandatoryKeywords>();
            await WriteRecord(record);
            }

        private bool WritingHeader => state == WriterState.PrimaryHeader || state == WriterState.SecondaryHeader;

        private bool WritingData => state == WriterState.PrimaryData || state == WriterState.SecondaryData;

        /// <summary>
        /// Writes an END record if not already present and flushes the block.
        /// Prepares to write a data block.
        /// </summary>
        /// <returns></returns>
        public async Task EndHeader()
            {
            if (headerRecords[EndKeyword].None)
                {
                var endRecord = FitsHeaderRecord.Create(EndKeyword);
                await WriteHeaderRecord(endRecord);
                await FlushBlock();
                state = state == WriterState.PrimaryHeader ? WriterState.PrimaryData : WriterState.SecondaryData;
                }
            }

        /// <summary>
        /// Asynchronously Writes an entire Single Image FITS file.
        /// </summary>
        /// <param name="hdu">
        ///  An instance of FitsHeaderDataUnit containing the header metadata and data array.
        /// </param>
        /// <returns>A Task that completes when the all of the data is written to the output stream.</returns>

        public async Task WriteHeaderDataUnit(FitsHeaderDataUnit hdu)
            {
            await WriteHeader(hdu.Header);
            await WriteDataArray(hdu.RawData, hdu.DataArrayLengthBytes);
            }

        /// <summary>
        /// Writes binary data into the current data unit and closes the data unit.
        /// The supplied bytes are written 'as-is' and not interpreted in any way.
        /// Padding characters will be appended if necessary to ensure that the output
        /// stream ends on a FITS block boundary.
        /// Does not close the writer, as the user may wish to add further header-data units.
        /// </summary>
        /// <param name="rawData">The memory buffer containing the data.</param>
        /// <param name="dataArrayLengthBytes">The number of bytes to be written.</param>
        /// <exception cref="FitsFormatException">Thrown if the writer is not in a valid state for writing a data array.</exception>
        private async Task WriteDataArray(byte[] rawData, int dataArrayLengthBytes)
            {
            if (WritingHeader)
                throw new FitsFormatException(
                    "Attempt to write data into a header block. Call EndHeader() before writing data.");
            if (!BlockIsEmpty)
                throw new FitsFormatException(
                    "Found unexpected unflushed data when writing a data array. Make sure you have completed any previous operations before writing a data array.");
            await stream.WriteAsync(rawData, 0, dataArrayLengthBytes);
            // Pad the stream as specified in FITS specification §3.3.2.
            var partialBlockWritten = dataArrayLengthBytes % FitsBlockLength;
            var paddingRequired = partialBlockWritten > 0 ? FitsBlockLength - partialBlockWritten : 0;
            while (paddingRequired-- > 0)
                {
                stream.WriteByte(0x00);
                }
            await EndDataBlock();
            }

        private async Task EndDataBlock()
            {
            currentBlock = CreateEmptyBlock();
            state = WriterState.SecondaryHeader;
            }

        private async Task WriteHeader(FitsHeader header)
            {
            if (WritingData) throw new FitsFormatException("Attempt to write header records into a data block");
            foreach (var headerRecord in header.HeaderRecords)
                {
                await WriteHeaderRecord(headerRecord);
                }
            await EndHeader();
            }

        public async Task WriteDataByte(byte datum)
            {
            if (WritingHeader) 
                throw new FitsFormatException(
                    "Attempt to write data bytes into a header block. Make sure to call EndHeader() before writing data");
            currentBlock.Append((char)datum);
            if (BlockSpaceRemaining <= 0)
                await WriteBlock();
            }
        }
    }