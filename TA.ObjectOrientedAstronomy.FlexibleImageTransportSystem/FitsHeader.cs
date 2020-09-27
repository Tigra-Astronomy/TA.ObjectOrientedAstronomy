// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeader.cs  Last modified: 2020-08-22@21:29 by Tim Long

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using TA.Utils.Core;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Represents an ordered collection of FITS header records, each of which has a mandatory keyword,
    ///     an optional value field and an optional comment field. The ordering of the header records is
    ///     preserved so that a FITS file may be read and re-written without disturbing the order of
    ///     existing records, or the records displayed in the order they occurred in the file.
    /// </summary>
    public class FitsHeader : IKeywordLookup<string, string>
        {
        private readonly SortedList<int, FitsHeaderRecord> records = new SortedList<int, FitsHeaderRecord>();
        private int nextSequenceNumber;

        /// <summary>
        ///     Gets a list of the header records in the order they occurred in the file, or in which they were
        ///     added.
        /// </summary>
        public IEnumerable<FitsHeaderRecord> HeaderRecords => records.Values.ToList();

        /// <summary>Gets the FITS header value for the specified keyword.</summary>
        /// <param name="keyword">The FITS header keyword to look up.</param>
        /// <returns>The value field of the first found match if one exists, as a <see cref="Maybe{T}" />.</returns>
        public Maybe<string> this[string keyword] =>
            records.Values.FirstOrDefault(p => p.Keyword == keyword)?.Value ?? Maybe<string>.Empty;

        /// <summary>Appends a header record, preserving the sequence of all existing records.</summary>
        /// <param name="record">The record to be appended.</param>
        /// <returns>The zero-based index of the added record.</returns>
        public int AppendHeaderRecord(FitsHeaderRecord record)
            {
            var index = Interlocked.Increment(ref nextSequenceNumber);
            records.Add(index, record);
            return index;
            }

        /// <summary>
        ///     Creates a header record collection containing the minimum required mandatory keywords for a
        ///     Single Image FITS file (SIF). The user may then append additional records before writing the
        ///     header out to a stream using a <see cref="FitsWriter" />.
        /// </summary>
        /// <param name="mandatoryKeywords">
        ///     Values that must be supplied as the bare minimum required for any
        ///     FITS file.
        /// </param>
        /// <returns>A <see cref="FitsHeader" /> instance populated with the required header records.</returns>
        public static FitsHeader CreateSingleImageFitsHeader(FitsPrimaryHduMandatoryKeywords mandatoryKeywords)
            {
            var header = new FitsHeader();
            header.AppendHeaderRecord(FitsHeaderRecord.Create("SIMPLE",
                Maybe<string>.From(mandatoryKeywords.Simple ? "T" : "F"),
                Maybe<string>.From("Single Image FITS file")));
            header.AppendHeaderRecord(FitsHeaderRecord.Create("BITPIX",
                Maybe<string>.From(mandatoryKeywords.BitsPerPixel.ToString(CultureInfo.InvariantCulture)),
                Maybe<string>.From("Pixel bit depth")));
            header.AppendHeaderRecord(FitsHeaderRecord.Create("NAXIS",
                Maybe<string>.From(mandatoryKeywords.NumberOfAxes.ToString(CultureInfo.InvariantCulture)),
                Maybe<string>.From("Number of axes")));
            for (var axis = 0; axis < mandatoryKeywords.LengthOfAxis.Count; axis++)
                {
                header.AppendHeaderRecord(FitsHeaderRecord.Create($"NAXIS{axis}",
                    Maybe<string>.From(mandatoryKeywords.LengthOfAxis[axis].ToString(CultureInfo.InvariantCulture)),
                    Maybe<string>.From($"Length of axis {axis}")));
                }
            return header;
            }
        }
    }