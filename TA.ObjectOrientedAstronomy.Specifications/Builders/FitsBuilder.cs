// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsBuilder.cs  Last modified: 2016-09-30@02:51 by Tim Long

using System;
using System.Text;
using static TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.Constants;

namespace TA.ObjectOrientedAstronomy.Specifications.Builders
    {
    class FitsBuilder
        {
        readonly StringBuilder builder = new StringBuilder();

        public FitsBuilder AppendRecord(string partialRecord)
            {
            if (partialRecord.Length > FitsRecordLength)
                throw new InvalidOperationException($"Records must not be more than {FitsRecordLength} characters");
            builder.Append(partialRecord);
            var recordUsed = partialRecord.Length;
            var recordUnused = FitsRecordLength - recordUsed;
            if (recordUsed > 0)
                builder.Append(' ', recordUnused);
            return this;
            }

        public string Build()
            {
            var blockUsed = builder.Length % FitsBlockLength;
            var blockUnused = FitsBlockLength - blockUsed;
            if (blockUsed > 0)
                builder.Append(' ', blockUnused);
            return builder.ToString();
            }

        public FitsBuilder AppendEmptyRecord()
            {
            builder.Append(' ', FitsRecordLength);
            return this;
            }
        }
    }