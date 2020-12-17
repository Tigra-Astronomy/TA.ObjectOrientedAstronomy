// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsKeywordAttribute.cs  Last modified: 2016-10-13@00:15 by Tim Long

using System;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder
    {
    /// <summary>
    ///     Class FitsKeywordAttribute. This class cannot be inherited. Used with property binding to identify the
    ///     header keyword that should provide the source data for a property.
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
        ///     Gets or sets the sequence number of this attribute. In situations where multiple attributes are applied
        ///     to the same property, this sequence number determines the order in which the attributes are considered.
        ///     Bindings occur in ascending order of sequence numbers. Attributes where the sequence number is not
        ///     specified will have a default of zero. For properties with equal sequence numbers, the order is
        ///     undefined and those properties may be used in any order relative to each other. Therefore it is
        ///     recommended that sequence numbers are always used when multiple attributes are applied to a property.
        /// </summary>
        /// <value>The sequence number.</value>
        public int Sequence { get; set; }

        /// <summary>
        /// Gets or sets the write sequence number.
        /// When serializing data transfer objects to FITS header record collections, this
        /// sequence number is used to determine the order in which records are created.
        /// Items having the same sequence number occur adjacent to each other in the header
        /// but not in any specific order.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// The following sequence numbers are reserved for mandatory and predefined headers:
        /// </para>
        ///   <para>
        ///     <br />
        ///   </para>
        ///   <list type="table">
        ///     <item>
        ///       <description>1</description>
        ///       <description>SIMPLE</description>
        ///     </item>
        ///     <item>
        ///       <description>2</description>
        ///       <description>BITPIX</description>
        ///     </item>
        ///     <item>
        ///       <description>3</description>
        ///       <description>NAXIS</description>
        ///     </item>
        ///     <item>
        ///       <description>4</description>
        ///       <description>NAXIS1</description>
        ///     </item>
        ///     <item>
        ///       <description>5</description>
        ///       <description>NAXIS2</description>
        ///     </item>
        ///     <item>
        ///       <description>6</description>
        ///       <description>NAXIS3</description>
        ///     </item>
        ///     <item>
        ///       <description></description>
        ///       <description></description>
        ///     </item>
        ///     <item>
        ///       <description>10</description>
        ///       <description>OBJECT</description>
        ///     </item>
        ///     <item>
        ///       <description>11</description>
        ///       <description>TELECOP</description>
        ///     </item>
        ///     <item>
        ///       <description>12</description>
        ///       <description>INSTRUME</description>
        ///     </item>
        ///     <item>
        ///       <description>13</description>
        ///       <description>OBSERVER</description>
        ///     </item>
        ///     <item>
        ///       <description>14</description>
        ///       <description>DATE-OBS</description>
        ///     </item>
        ///     <item>
        ///       <description>15</description>
        ///       <description>BSCALE</description>
        ///     </item>
        ///     <item>
        ///       <description>16</description>
        ///       <description>BZERO</description>
        ///     </item>
        ///     <item>
        ///       <description>9999</description>
        ///       <description>HISTORY</description>
        ///     </item>
        ///     <item>
        ///       <description></description>
        ///       <description></description>
        ///     </item>
        ///     <item>
        ///       <description></description>
        ///       <description></description>
        ///     </item>
        ///     <item>
        ///       <description></description>
        ///       <description></description>
        ///     </item>
        ///     <item>
        ///       <description></description>
        ///       <description></description>
        ///     </item>
        ///     <item>
        ///       <description></description>
        ///       <description></description>
        ///     </item>
        ///   </list>
        ///   <para>
        ///     <br />
        ///   </para>
        /// </remarks>
        public int WriteSequence { get; set; }
        }
    }