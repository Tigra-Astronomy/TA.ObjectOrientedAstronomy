// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeader.cs  Last modified: 2016-09-29@18:35 by Tim Long

using System.Collections.Generic;
using System.Linq;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Threading;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    [ReaderWriterSynchronized]
    public class FitsHeader
        {
        [Reference] private readonly SortedList<int, FitsHeaderRecord> records = new SortedList<int, FitsHeaderRecord>();
        private int nextSequenceNumber;

        public IEnumerable<FitsHeaderRecord> HeaderRecords => records.Values.ToList();

        [Writer]
        public int AppendHeaderRecord(FitsHeaderRecord record)
            {
            var index = nextSequenceNumber++;
            records.Add(index, record);
            return index;
            }
        }
    }