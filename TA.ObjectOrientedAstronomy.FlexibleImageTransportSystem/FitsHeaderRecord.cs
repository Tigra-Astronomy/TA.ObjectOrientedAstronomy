// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeaderRecord.cs  Last modified: 2016-10-13@23:40 by Tim Long

using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public sealed class FitsHeaderRecord : FitsRecord
        {
        private static readonly Regex headerParser = new Regex(FitsFormat.FitsHeaderRecordPattern, RegexOptions.Compiled);
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private FitsHeaderRecord(string record)
            {
            Text = record;
            Keyword = Value = Comment = string.Empty;
            }

        public string Keyword { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }

        public static FitsHeaderRecord Empty { get; } =
            new FitsHeaderRecord(new string(' ', FitsFormat.FitsRecordLength))
            ;

        public override string ToString()
            {
            return $"{Keyword} = {Value} / {Comment}";
            }

        /// <summary>
        ///     Creates a new <see cref="FitsHeaderRecord" /> from record text in the supplied string.
        /// </summary>
        /// <param name="text">The record text, which must contain exactly 80 permitted ASCII characters.</param>
        /// <returns>A new instance of <see cref="FitsHeaderRecord" /> initialized from the record text.</returns>
        public static FitsHeaderRecord FromRecordText(string text)
            {
            Contract.Requires(!string.IsNullOrEmpty(text));
            Contract.Ensures(Contract.Result<FitsHeaderRecord>() != null);
            Log.Info("Parsing FITS Header Record: {0}", text);
            if (text.Length != 80)
                throw new FitsFormatException(
                    $"Found {text.Length} characters. FITS records must contain exactly 80 characters")
                    {
                    Record = text
                    };
            var equalsPosition = text.IndexOf(FitsFormat.EqualsCharacter);
            var commentPosition = text.IndexOf(FitsFormat.CommentCharacter);
            var record = new FitsHeaderRecord(text);
            /*
             * If the first non blank character is the comment character then the record is a comment. The
             * comment text begins at the first non blank character after the comment character.
             */
            if (text.TrimStart().FirstOrDefault() == FitsFormat.CommentCharacter)
                {
                record.Comment = ParseComment(text);
                record.Keyword = string.Empty;
                record.Value = string.Empty;
                return record;
                }
            record.Keyword = ParseKeyword(text);
            if (record.Keyword.IsCommentary() || text.HasNoValue())
                {
                // keywords that are blank, or one of the commentary types, or which have no value
                // have comment text from position 9 and no value.
                record.Comment = text.RemoveHead(FitsFormat.ValueFieldPosition).TrimEnd();
                record.Value = string.Empty;
                }
            else
                {
                record.Value = ParseValueField(text);
                record.Comment = ParseComment(text);
                }
            return record;
            }

        private static string ParseComment(string recordText)
            {
            Contract.Requires(!string.IsNullOrEmpty(recordText));
            Contract.Requires(recordText.Length == FitsFormat.FitsRecordLength);
            var commentPosition = recordText.IndexOf(FitsFormat.CommentCharacter);
            if (commentPosition < 0)
                return string.Empty;
            return recordText.RemoveHead(++commentPosition).Trim();
            }

        private static string ParseValueField(string recordText)
            {
            Contract.Requires(!string.IsNullOrEmpty(recordText));
            Contract.Requires(recordText.Length > FitsFormat.ValueFieldPosition);
            var valueField = recordText.RemoveHead(FitsFormat.ValueFieldPosition);
            var commentPosition = valueField.IndexOf(FitsFormat.CommentCharacter);
            if (commentPosition == 0)
                return string.Empty; // it's all comment
            if (commentPosition < 0)
                return valueField.Trim();
            return valueField.Head(commentPosition).Trim();
            }

        private static string ParseKeyword(string record)
            {
            Contract.Requires(!string.IsNullOrEmpty(record));
            Contract.Requires(record.Length == FitsFormat.FitsRecordLength);
            var keyword = record.Head(FitsFormat.KeywordLength).Trim();
            if (!keyword.ContainsOnly(FitsFormat.KeywordPermittedCharacters))
                throw new FitsFormatException($"Keyword [{keyword}] contained forbidden characters", keyword)
                    {
                    Data = {{"Permitted Characters", FitsFormat.KeywordPermittedCharacters}}
                    };
            return keyword;
            }
        }
    }