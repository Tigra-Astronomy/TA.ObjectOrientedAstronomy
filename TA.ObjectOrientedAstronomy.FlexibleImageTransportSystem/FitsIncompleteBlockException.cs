// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsIncompleteBlockException.cs  Last modified: 2016-10-07@03:15 by Tim Long

using System;
using System.Runtime.Serialization;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     An exception thrown by <see cref="FitsReader" /> if it is unable to read a complete block of data from a
    ///     FITS data stream.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class FitsIncompleteBlockException : Exception
        {
        private static readonly string defaultMessage =
            $"Unable to read a complete FITS block of {Constants.FitsBlockLength} bytes. The FITS specification states that all FITS files must contain an integer number of blocks each of {Constants.FitsBlockLength} bytes. Header and Data Units should be padded if necessary to form whole blocks, including at the end of the file. This may be an indication of file corruption or an application that does not properly conform to the FITS standard. It may be safe to ignore this error if it is at the end of the file.";

        public FitsIncompleteBlockException() : base(defaultMessage) {}

        public FitsIncompleteBlockException(Exception inner) : base(defaultMessage, inner) {}

        protected FitsIncompleteBlockException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
        }
    }