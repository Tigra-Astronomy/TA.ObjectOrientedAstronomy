// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsWriter.cs  Last modified: 2020-08-20@14:58 by Tim Long

using System.IO;
using System.Text;
using System.Threading.Tasks;
using static TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.FitsFormat;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public class FitsWriter
        {
        readonly Stream stream;
        internal StringBuilder currentBlock;

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

        internal async Task FlushBlock()
            {
            if (BlockIsEmpty) return;
            var bytesToPad = BlockSpaceRemaining;
            if (bytesToPad > 0)
                currentBlock.Append('\0', bytesToPad);
            await WriteBlock();
            }
        }
    }