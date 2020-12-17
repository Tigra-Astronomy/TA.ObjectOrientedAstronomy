// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsRecord.cs  Last modified: 2016-10-12@23:53 by Tim Long

using System;
using System.Text;
using static TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.FitsFormat;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public class FitsRecord
        {
        // ToDo: the storage unit should probably be byte[] rather than string.
        public string Text { get; protected set; }

        /// <summary>
        ///     Creates a FITS record from a buffer containing ASCII bytes.
        /// </summary>
        /// <param name="asciiBytes">
        ///     A byte array containing exactly <see cref="RecordLength" /> ASCII-encoded characters.
        /// </param>
        /// <returns>A new <see cref="FitsRecord" /> initialized with the ASCII data.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if the supplied buffer does not contain
        ///     <see cref="RecordLength" /> bytes.
        /// </exception>
        public static FitsRecord FromAsciiBytes(byte[] asciiBytes)
            {
            if (asciiBytes.Length != FitsRecordLength)
                throw new ArgumentException(
                    $"FITS records require exactly {FitsRecordLength} bytes (received {asciiBytes.Length})",
                    nameof(asciiBytes));
            var utf8 = Encoding.ASCII.GetString(asciiBytes);
            return new FitsRecord {Text = utf8};
            }

        public static FitsRecord FromMemory(Memory<byte> memory)
            {
            if (memory.Length != FitsRecordLength)
                throw new ArgumentException(
                    $"FITS records require exactly {FitsRecordLength} bytes (received {memory.Length})",
                    nameof(memory));
            var bytes = memory.ToArray();
            return FromAsciiBytes(bytes);
            }

        public static FitsRecord FromString(string source)
            {
            var sourceLength = source.Length;
            if (sourceLength != FitsRecordLength)
                throw new ArgumentException($"The supplied string must be exactly {FitsRecordLength} characters but was {sourceLength}",
                    nameof(source));
            return new FitsRecord {Text = source};
            }
        }
    }