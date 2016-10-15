// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: StringExtensions.cs  Last modified: 2016-10-15@04:16 by Tim Long

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
    {
    /// <summary>
    ///     String extension methods.
    /// </summary>
    /// <remarks>
    ///     Internal use only. Driver and application developers should not rely on this class, because the interface
    ///     and method signatures may change at any time.
    /// </remarks>
    public static class StringExtensions
        {
        /// <summary>
        ///     Returns the specified number of characters from the head of a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">The number of characters to be returned, must not be greater than the length of the string.</param>
        /// <returns>The specified number of characters from the head of the source string, as a new string.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     Thrown if the requested number of characters exceeds the string
        ///     length.
        /// </exception>
        [NotNull]
        public static string Head(this string source, int length)
            {
            Contract.Requires(source != null);
            Contract.Requires(length <= source.Length,
                "The specified length is greater than the length of the string.");
            Contract.Requires<ArgumentOutOfRangeException>(length >= 0, "The specified length must be positive");
            Contract.Ensures(Contract.Result<string>() != null);
            return source.Substring(0, length);
            }

        /// <summary>
        ///     Returns the specified number of characters from the tail of a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">The number of characters to be returned, must not be greater than the length of the string.</param>
        /// <returns>The specified number of characters from the tail of the source string, as a new string.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     Thrown if the requested number of characters exceeds the string
        ///     length.
        /// </exception>
        [NotNull]
        public static string Tail(this string source, int length)
            {
            Contract.Requires(source != null);
            Contract.Requires(length <= source.Length, "The specified length is greater than the length of the string.");
            Contract.Requires(length >= 0, "length must be positive");
            Contract.Ensures(Contract.Result<string>() != null);
            return source.Substring(source.Length - length, length);
            }

        /// <summary>
        ///     Keeps only the wanted (that is, removes all unwanted characters) from the string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="keep">A potentially empty list of the wanted characters. All other characters will be removed.</param>
        /// <returns>
        ///     A new string with all of the unwanted characters deleted.
        ///     Returns <see cref="string.Empty" /> if all the characters were deleted or if the source string was null or empty.
        /// </returns>
        /// <seealso cref="Clean" />
        [NotNull]
        public static string Keep(this string source, string keep)
            {
            Contract.Requires(keep != null);
            Contract.Ensures(Contract.Result<string>() != null);
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var cleanString = new StringBuilder(source.Length);
            foreach (var ch in source)
                {
                if (keep.Contains(ch))
                    cleanString.Append(ch);
                }
            return cleanString.ToString();
            }

        /// <summary>
        ///     Determines whether the specified source contains only allowed characters.
        ///     A null or empty string is always considered to contain only allowed characters,
        ///     by definition.
        /// </summary>
        /// <param name="source">The source string to be tested.</param>
        /// <param name="allowedCharacters">The allowed characters.</param>
        /// <returns><c>true</c> if the specified source contains only allowed characters; otherwise, <c>false</c>.</returns>
        public static bool ContainsOnly(this string source, string allowedCharacters)
            {
            if (string.IsNullOrEmpty(source))
                return true;
            foreach (var ch in source)
                {
                if (allowedCharacters.Contains(ch))
                    continue;
                return false;
                }
            return true;
            }

        /// <summary>
        ///     Removes all unwanted characters from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="clean">A list of the unwanted characters. All other characters will be preserved.</param>
        /// <returns>
        ///     A string with all of the unwanted characters deleted. Returns <see cref="string.Empty" />
        ///     if all of the characters were deleted or if the source string was null or empty.
        ///     Returns the unmodified source string if <paramref name="clean" /> was null or empty.
        ///     The result is guaranteed not to be null.
        /// </returns>
        /// <seealso cref="Keep" />
        /// <remarks>
        ///     Contrast with <see cref="Keep" />
        /// </remarks>
        /// <seealso cref="Keep" />
        [NotNull]
        public static string Clean(this string source, string clean)
            {
            Contract.Ensures(Contract.Result<string>() != null);
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            if (string.IsNullOrEmpty(clean))
                return source;
            var cleanString = new StringBuilder(source.Length);
            foreach (var ch in source)
                {
                if (!clean.Contains(ch))
                    cleanString.Append(ch);
                }
            return cleanString.ToString();
            }

        /// <summary>
        ///     Remove the head of the string, leaving the tail.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">Number of characters to remove from the head.</param>
        /// <returns>A new string containing the old string with <paramref name="length" /> characters removed from the head.</returns>
        public static string RemoveHead(this string source, int length)
            {
            if (length < 1)
                return source;
            return source.Tail(source.Length - length);
            }

        /// <summary>
        ///     Remove the tail of the string, leaving the head.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">Number of characters to remove from the tail.</param>
        /// <returns>A new string containing the old string with <paramref name="length" /> characters removed from the tail.</returns>
        public static string RemoveTail(this string source, int length)
            {
            if (length < 1)
                return source;
            return source.Head(source.Length - length);
            }

        /// <summary>
        ///     Converts a string to a hex representation, suitable for display in a debugger.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>A new string showing each character of the source string as a hex digit.</returns>
        public static string ToHex(this string source)
            {
            Contract.Ensures(Contract.Result<string>() != null);
            const string formatWithSeperator = ", {0,2:x}";
            const string formatNoSeperator = "{0,2:x}";
            if (source == null)
                return "{null}";
            if (string.IsNullOrEmpty(source))
                return "{empty}";
            var hexString = new StringBuilder(source.Length * 7);
            hexString.Append('{');
            var seperator = false;
            foreach (var ch in source)
                {
                hexString.AppendFormat(seperator ? formatWithSeperator : formatNoSeperator, (int) ch);
                seperator = true;
                }
            hexString.Append('}');
            return hexString.ToString();
            }

        /// <summary>
        ///     Determines whether this string contains the specified substring,
        ///     using the
        ///     <see cref="System.StringComparison.InvariantCultureIgnoreCase" />
        ///     comparer.
        /// </summary>
        /// <param name="containingString">The string to be examined.</param>
        /// <param name="substring">The substring to be detected.</param>
        /// <returns>
        ///     <c>true</c> if the string contains the specified substring;
        ///     otherwise, <c>false</c> .
        /// </returns>
        public static bool ContainsCaseInsensitive(this string containingString, string substring)
            {
            Contract.Requires(containingString != null);
            Contract.Requires(substring != null);
            return containingString.IndexOf(substring, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
        }
    }