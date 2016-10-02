// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsKeywordAttribute.cs  Last modified: 2016-10-02@01:15 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder
    {
    /// <summary>
    ///     Class FitsKeywordAttribute. This class cannot be inherited. Used with property binding to identify the key
    ///     name (as it occurs in the Gemini response payload) that will provide the source data for a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FitsKeywordAttribute : Attribute
        {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FitsKeywordAttribute" /> class.
        /// </summary>
        /// <param name="keyword">
        ///     The name of the keyword, as it appears in the FITS header, that will supply the data for
        ///     the decorated property.
        /// </param>
        public FitsKeywordAttribute(string keyword)
            {
            Sequence = 0;
            Keyword = keyword;
            }

        /// <summary>
        ///     Gets the FITS keyword.
        /// </summary>
        /// <value>The keyword.</value>
        public string Keyword { get; }

        /// <summary>
        ///     Gets or sets the sequence number of this attribute. The sequence number is an optional, named parameter
        ///     which affects the order that keywords are mapped against a complex (collection) property. Mappings occur in
        ///     ascending order of sequence numbers. For properties with equal sequence numbers, the order is undefined.
        /// </summary>
        /// <value>The sequence number.</value>
        public int Sequence { get; set; }
        }
    }