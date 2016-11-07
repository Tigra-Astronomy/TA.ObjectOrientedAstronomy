// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsIncompleteBlockException.cs  Last modified: 2016-11-07@19:09 by Tim Long

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
    public class FitsIncompleteBlockException : FitsFormatException
        {
        private static readonly string defaultMessage =
            $"Unable to read a complete FITS block of {FitsFormat.FitsBlockLength} bytes. The FITS specification states that all FITS files must contain an integer number of blocks each of {FitsFormat.FitsBlockLength} bytes. Header and Data Units should be padded if necessary to form whole blocks, including at the end of the file. This may be an indication of file corruption or an application that does not properly conform to the FITS standard. It may be safe to ignore this error if it is at the end of the file.";

        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsIncompleteBlockException" /> class.
        /// </summary>
        public FitsIncompleteBlockException() : base(defaultMessage) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsIncompleteBlockException" /> class by wrapping an inner
        ///     exception.
        /// </summary>
        /// <param name="inner">The inner exception.</param>
        public FitsIncompleteBlockException(Exception inner) : base(defaultMessage, inner) {}

        /// <summary>
        ///     Serialization constructor, used by the system when an exception must be passed across a remoting channel.
        ///     This is not expected to be called by the user and therefore is protected.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        protected FitsIncompleteBlockException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
        }
    }