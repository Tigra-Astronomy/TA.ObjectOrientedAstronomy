using System;
using System.Collections.Generic;
using System.Text;
using TA.Utils.Core;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
{
    /// <summary>
    /// Extension methods for manipulating FITS headers.
    /// </summary>
    public static class FitsHeaderExtensions
    {
        /// <summary>
        /// Appends a history record to the header.
        /// </summary>
        /// <param name="header">The FITS header.</param>
        /// <param name="comment">The comment to be appended.</param>
        public static void AppendHistory(this FitsHeader header, string comment)
        {
        var history = FitsHeaderRecord.Create("HISTORY", Maybe<string>.Empty, comment.AsMaybe());
        header.AppendHeaderRecord(history);
        }
    }
}
