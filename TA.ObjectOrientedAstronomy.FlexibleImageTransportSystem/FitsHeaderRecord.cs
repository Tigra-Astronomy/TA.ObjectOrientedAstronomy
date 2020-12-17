// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeaderRecord.cs  Last modified: 2020-08-22@17:10 by Tim Long

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TA.ObjectOrientedAstronomy.FundamentalTypes;
using TA.Utils.Core;
using TA.Utils.Core.Diagnostics;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Represents a single FITS header record, in a form that can be read from or written to a FITS
    ///     file. FITS records must always contain exactly <see cref="FitsFormat.FitsRecordLength" />
    ///     characters.
    /// </summary>
    public sealed class FitsHeaderRecord : FitsRecord
        {
        private static readonly Regex headerParser =
            new Regex(FitsFormat.FitsHeaderRecordPattern, RegexOptions.Compiled);

        public static Regex FitsKeywordRegex = new Regex(FitsFormat.FitsValidKeywordPattern,
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private FitsHeaderRecord(string record)
            {
            Text = record;
            Keyword = string.Empty;
            Value = Comment = Maybe<string>.Empty;
            }

        public string Keyword { get; set; }

        public Maybe<string> Value { get; set; }

        public Maybe<string> Comment { get; set; }

        public static FitsHeaderRecord Empty { get; } =
            new FitsHeaderRecord(new string(' ', FitsFormat.FitsRecordLength));

        /// <summary>
        ///     Creates a FITS Header Record from the specified keyword plus optional value and comment,
        ///     which complies with § 4 of the NASA FITS Standard.
        /// </summary>
        /// <param name="keyword">
        ///     The FITS keyword, which is required and must be a valid keyword as defined in § 4.1.2.1 of the
        ///     FITS standard, with one exception: mixed case will be accepted here. FITS keywords must be
        ///     upper-case, so any lower-case characters will be converted to upper-case.
        ///     The keyword should not contain any padding or white space.
        /// </param>
        /// <param name="value">
        ///     An optional value string to be written to the value field of the header record. If a value is
        ///     present, then the Value Indicator (§4.1.2.2) will be added automatically. The value string must
        ///     conform to FITS standard § 4.2.
        ///     The value should not contain any padding or (except within string values) white space.
        /// </param>
        /// <param name="comment">
        ///     An optional comment to be written in the comment field of the header record. The
        ///     <see cref="FitsFormat.CommentCharacter" /> should not be included in the string and will be
        ///     added automatically if the comment is present. The string must conform to FITS standard §
        ///     4.1.2.3.
        /// </param>
        /// <exception cref="FitsFormatException">
        ///     Thrown if any of the supplied parameters are invalid according to FITS standard § 4.
        /// </exception>
        public static FitsHeaderRecord Create(string keyword, Maybe<string> value, Maybe<string> comment)
            {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new FitsFormatException("The keyword must not be null, empty or contain non-printing characters");
            var keywordLength = keyword.Length;
            if (keywordLength > 8)
                throw new FitsFormatException(
                    $"The supplied keyword had {keywordLength} characters; the maximum allowed is {FitsFormat.KeywordLength} [FITS Standard § 4.1.2.1]");
            var fitsKeyword = keyword.ToUpper(CultureInfo.InvariantCulture);
            if (!FitsKeywordRegex.IsMatch(fitsKeyword))
                throw new FitsFormatException($"Keyword '{fitsKeyword}' is not a valid FITS header keyword [FITS standard § 4.1.2.1]");
            var builder = new StringBuilder(fitsKeyword);
            if (keywordLength < 8)
                builder.Append(FitsFormat.PadCharacter, 8 - keywordLength);
            if (value.Any())
                builder.Append("= ").Append(value.Single());
            if (comment.Any())
                builder.Append(value.Any() ? " / " : "/ ").Append(comment.Single());
            var charsUsed = builder.Length;
            if (charsUsed < FitsFormat.FitsRecordLength)
                builder.Append(FitsFormat.PadCharacter, FitsFormat.FitsRecordLength - charsUsed);
            return new FitsHeaderRecord(builder.ToString()) {Keyword = fitsKeyword, Value = value, Comment = comment};
            }

        /// <summary>
        /// Creates a FITS Header Record from the supplied non-null non-empty arguments.
        /// </summary>
        /// <param name="keyword">The FITS keyword, which must not be null or empty.</param>
        /// <param name="value">The record value, which must not be null or empty.</param>
        /// <param name="comment">The comment field, which must not be null or empty.</param>
        /// <returns></returns>
        public static FitsHeaderRecord Create(string keyword, string value, string comment) => Create(keyword, value.AsMaybe(), comment.AsMaybe());

        /// <summary>
        /// Creates a FITS Keyword-Value Header Record from the supplied non-null non-empty arguments.
        /// </summary>
        /// <param name="keyword">The FITS keyword, which must not be null or empty.</param>
        /// <param name="value">The record value, which must not be null or empty.</param>
        /// <returns></returns>
        public static FitsHeaderRecord Create(string keyword, string value) => Create(keyword, value.AsMaybe(), Maybe<string>.Empty);

        /// <summary>
        /// Creates a keyword-only FITS Header Record from the supplied keyword.
        /// </summary>
        /// <param name="keyword">The FITS keyword, which must not be null or empty.</param>
        /// <returns></returns>
        public static FitsHeaderRecord Create(string keyword) => Create(keyword, Maybe<string>.Empty, Maybe<string>.Empty);

        public override string ToString()
            {
            return $"{Keyword,-8} = {Value} / {Comment}";
            }

        /// <summary>Creates a new <see cref="FitsHeaderRecord" /> from record text in the supplied string.</summary>
        /// <param name="text">The record text, which must contain exactly 80 permitted ASCII characters.</param>
        /// <returns>A new instance of <see cref="FitsHeaderRecord" /> initialized from the record text.</returns>
        public static FitsHeaderRecord FromRecordText(string text, ILog log = null)
            {
            Contract.Requires(!string.IsNullOrEmpty(text));
            Contract.Ensures(Contract.Result<FitsHeaderRecord>() != null);
            log?.Info().Message("Parsing FITS Header Record: {text}", text).Write();
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
                record.Value = Maybe<string>.Empty;
                return record;
                }
            record.Keyword = ParseKeyword(text);
            if (record.Keyword.IsCommentary() || text.HasNoValue())
                {
                // keywords that are blank, or one of the commentary types, or which have no value
                // have comment text from position 9 and no value.
                record.Comment = text.RemoveHead(record.Keyword.Length + 1).TrimEnd().AsMaybe();
                record.Value = Maybe<string>.Empty;
                }
            else
                {
                record.Value = ParseValueField(text);
                record.Comment = ParseComment(text);
                }
            return record;
            }

        private static Maybe<string> ParseComment(string recordText)
            {
            Contract.Requires(!string.IsNullOrEmpty(recordText));
            Contract.Requires(recordText.Length == FitsFormat.FitsRecordLength);
            var commentPosition = recordText.IndexOf(FitsFormat.CommentCharacter);
            if (commentPosition < 0)
                return Maybe<string>.Empty;
            return recordText.RemoveHead(++commentPosition).Trim().AsMaybe();
            }

        private static Maybe<string> ParseValueField(string recordText)
            {
            Contract.Requires(!string.IsNullOrEmpty(recordText));
            Contract.Requires(recordText.Length > FitsFormat.ValueFieldPosition);
            if (recordText.HasNoValue())
                return Maybe<string>.Empty; // No value indicator present, therefore there can be no value.
            var valueField = recordText.RemoveHead(FitsFormat.ValueFieldPosition);
            var commentPosition = valueField.IndexOf(FitsFormat.CommentCharacter);
            if (commentPosition == 0
            ) // The value field starts with a comment. This is an error condition as we were expecting a value.
                throw new InvalidHeaderRecordException(
                    "A record with a value indicator should contain a value, but was all comment", recordText);
            if (commentPosition < 0)
                return valueField.Trim().AsMaybe(); // There's no comment, so its all value
            return valueField.Head(commentPosition).Trim().AsMaybe();
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