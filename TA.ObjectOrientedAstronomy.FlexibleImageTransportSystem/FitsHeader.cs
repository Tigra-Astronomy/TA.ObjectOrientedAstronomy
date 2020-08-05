// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeader.cs  Last modified: 2016-10-02@06:40 by Tim Long

using System.Collections.Generic;
using System.Linq;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Threading;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;
using TA.Utils.Core;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    [ReaderWriterSynchronized]
    public class FitsHeader : IKeywordLookup<string, string>
        {
        [Reference] private readonly SortedList<int, FitsHeaderRecord> records = new SortedList<int, FitsHeaderRecord>();
        private int nextSequenceNumber;

        public IEnumerable<FitsHeaderRecord> HeaderRecords => records.Values.ToList();

        /// <summary>
        ///     Gets the FITS header value for the specified keyword.
        /// </summary>
        /// <param name="keyword">The FITS header keyword.</param>
        /// <returns>The value field of the first found match, or <see cref="string.Empty" />.</returns>
        public Maybe<string> this[string keyword]
            => Maybe<string>.From(records.Values.FirstOrDefault(p => p.Keyword == keyword)?.Value);

        [Writer]
        public int AppendHeaderRecord(FitsHeaderRecord record)
            {
            var index = nextSequenceNumber++;
            records.Add(index, record);
            return index;
            }
        }
    }