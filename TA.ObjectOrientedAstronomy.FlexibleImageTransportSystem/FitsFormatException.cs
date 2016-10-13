// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsFormatException.cs  Last modified: 2016-10-13@22:51 by Tim Long

using System;
using JetBrains.Annotations;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Exception thrown when a FITS file is not in a valid format
    /// </summary>
    /// <remarks>
    ///     This exception does not have any custom properties, thus it does not implement ISerializable.
    /// </remarks>
    [Serializable]
    public sealed class FitsFormatException : Exception
        {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsFormatException" /> class.
        /// </summary>
        public FitsFormatException() : base("The FITS data was in an invalid format") {}

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

        [StringFormatMethod("message")]
        public FitsFormatException(string message, string keyword) : base(message)
            {
            Keyword = keyword;
            Data["Keyword"] = keyword;
            }

        public string File { get; set; }

        public string Keyword { get; set; }

        public string Record { get; set; }
        }
    }