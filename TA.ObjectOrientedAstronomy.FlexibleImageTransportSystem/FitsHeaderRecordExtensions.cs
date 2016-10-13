// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeaderRecordExtensions.cs  Last modified: 2016-10-13@23:02 by Tim Long

using System.Linq;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public static class FitsHeaderRecordExtensions
        {
        /// <summary>
        ///     Determines whether the specified FITS record has a value field.
        /// </summary>
        /// <param name="record">The FITS record to examine.</param>
        /// <returns><c>true</c> if the record has a value field; otherwise, <c>false</c>.</returns>
        public static bool HasValue(this string record)
            {
            if (string.IsNullOrEmpty(record))
                return false;
            if (record.Length <= FitsFormat.ValueIndicatorPosition)
                return false;
            return record.Substring(FitsFormat.ValueIndicatorPosition, FitsFormat.ValueIndicator.Length) ==
                   FitsFormat.ValueIndicator;
            }

        /// <summary>
        ///     Determines whether the value field is absent for the given FITS record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns><c>true</c> if the value field is absent; otherwise, <c>false</c>.</returns>
        public static bool HasNoValue(this string record)
            {
            return !record.HasValue();
            }

        /// <summary>
        ///     Determines whether the specified header keyword is a Commentary type.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns><c>true</c> if the specified keyword is commentary; otherwise, <c>false</c>.</returns>
        public static bool IsCommentary(this string keyword)
            {
            if (string.IsNullOrWhiteSpace(keyword))
                return true;
            return FitsFormat.CommentaryKeywords.Contains(keyword);
            }
        }
    }