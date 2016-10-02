// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeaderRecord.cs  Last modified: 2016-10-02@03:49 by Tim Long

using System.Text.RegularExpressions;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public class FitsHeaderRecord : FitsRecord
        {
        private static readonly Regex headerParser = new Regex(Constants.FitsHeaderRecordPattern, RegexOptions.Compiled);

        private FitsHeaderRecord(string record)
            {
            Text = record;
            Keyword = Value = Comment = string.Empty;
            }

        public string Keyword { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }

        public static FitsHeaderRecord Empty { get; } = new FitsHeaderRecord(new string(' ', Constants.FitsRecordLength))
            ;

        /// <summary>
        ///     Creates a new <see cref="FitsHeaderRecord" /> from record text in the supplied string.
        /// </summary>
        /// <param name="record">The raw record text, <see cref="Constants.FitsRecordLength" /> characters.</param>
        /// <returns>A new <c>FitsHeaderRecord</c> initialized with values parsed from the source record.</returns>
        /// <exception cref="InvalidHeaderRecordException">Unable to parse</exception>
        public static FitsHeaderRecord FromString(string record)
            {
            var header = new FitsHeaderRecord(record);
            var matches = headerParser.Match(record);
            if (!matches.Success)
                throw new InvalidHeaderRecordException("Unable to parse", record);
            header.Keyword = matches.Groups["Keyword"].Value ?? string.Empty;
            header.Value = matches.Groups["Value"].Value.Trim() ?? string.Empty;
            header.Comment = matches.Groups["Comment"].Value.Trim() ?? string.Empty;
            return header;
            }

        public override string ToString()
            {
            return $"{Keyword} = {Value} / {Comment}";
            }
        }
    }