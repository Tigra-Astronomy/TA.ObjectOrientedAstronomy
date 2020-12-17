// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: FitsSerializeSequenceAttribute.cs  Last modified: 2020-12-12@14:53 by Tim Long

using System;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    /// An attribute used to control various serialization options.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class FitsSerializeSequenceAttribute : Attribute
        {
        /// <summary>
        /// Gets the sequence number, which is used to determine the order in which
        /// header records are serialized. Items having the same sequence number
        /// will be adjacent to each other in the headers but in an undefined order.
        /// Items without this attribute occur towards the end of the header.
        /// </summary>
        public int Sequence { get; }

        /// <summary>
        /// Gets or sets the FITS keyword to be written into the header for a property.
        /// If this optional item is not specified, then a value from a
        /// <seealso cref="FitsKeywordAttribute"/> will be used if present,
        /// otherwise the header keyword will be derived from the property name.
        /// </summary>
        public string Keyword { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether to append an index number
        /// to the FITS keyword for each item in a collection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [append index]; otherwise, <c>false</c>.
        /// </value>
        public bool AppendIndex { get; set; } = false;

        public FitsSerializeSequenceAttribute(int sequence)
            {
            Sequence = sequence;
            }
        }
    }