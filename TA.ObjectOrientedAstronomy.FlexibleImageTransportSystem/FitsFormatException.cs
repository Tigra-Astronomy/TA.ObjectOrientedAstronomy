// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsFormatException.cs  Last modified: 2016-11-07@19:07 by Tim Long

using System;
using System.Runtime.Serialization;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Exception thrown when a FITS file is not in a valid format
    /// </summary>
    /// <remarks>
    ///     This exception does not have any custom properties, thus it does not implement ISerializable.
    /// </remarks>
    [Serializable]
    public class FitsFormatException : Exception
        {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsFormatException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FitsFormatException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsFormatException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FitsFormatException(string message) : base(message) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsFormatException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="keyword">The FITS header keyword related to the exception.</param>
        public FitsFormatException(string message, string keyword) : base($"[{keyword}]: {message}")
            {
            Keyword = keyword;
            Data["Keyword"] = keyword;
            }

        /// <summary>
        ///     Serialization constructor, used by the system when an exception must be passed across a remoting channel.
        ///     This is not expected to be called by the user and therefore is protected.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        protected FitsFormatException(SerializationInfo info, StreamingContext context) : base(info, context) {}


        public string File { get; set; }

        public string Keyword { get; set; }

        public string Record { get; set; }
        }
    }