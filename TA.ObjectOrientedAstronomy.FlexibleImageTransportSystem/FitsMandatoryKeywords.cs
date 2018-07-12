// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsMandatoryKeywords.cs  Last modified: 2016-10-02@07:25 by Tim Long

using System.Collections.Generic;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public class FitsMandatoryKeywords
        {
        /// <summary>
        ///     Gets or sets the number of axes in the data array, as specified in the NAXIS header record.
        /// </summary>
        /// <value>The number of axes.</value>
        [FitsKeyword("NAXIS")]
        public int NumberOfAxes { get; set; }

        /// <summary>
        ///     Gets or sets the length of each axis, as specified in the NAXISn header records, for up to 9 axes. A
        ///     correctly formed FITS file will only have NAXISn entries from 1 up to the number of axes specified by
        ///     <see cref="NumberOfAxes" />. Axes are numbered from 1 in FITS files, so the first element (element [0]) of
        ///     the array corresponds to axis 1.
        /// </summary>
        /// <value>
        ///     An array containing the length of each axis. Element [0] in the array corresponds to NAXIS1 and so
        ///     on.
        /// </value>
        [FitsKeyword("NAXIS1", Sequence = 1)]
        [FitsKeyword("NAXIS2", Sequence = 2)]
        [FitsKeyword("NAXIS3", Sequence = 3)]
        [FitsKeyword("NAXIS4", Sequence = 4)]
        [FitsKeyword("NAXIS5", Sequence = 5)]
        [FitsKeyword("NAXIS6", Sequence = 6)]
        [FitsKeyword("NAXIS7", Sequence = 7)]
        [FitsKeyword("NAXIS8", Sequence = 8)]
        [FitsKeyword("NAXIS9", Sequence = 9)]
        public List<int> LengthOfAxis { get; set; } = new List<int>();

        /// <summary>
        ///     Gets or sets the number of bits per pixel in the data array. The valid values are as follows:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Value</term>
        ///             <description>Interpretation</description>
        ///         </listheader>
        ///         <item>
        ///             <term>8</term>
        ///             <description>Character or unsigned binary integer</description>
        ///         </item>
        ///         <item>
        ///             <term>16</term>
        ///             <description>16-bit two's complement binary integer</description>
        ///         </item>
        ///         <item>
        ///             <term>32</term>
        ///             <description>32-bit two's complement binary integer</description>
        ///         </item>
        ///         <item>
        ///             <term>64</term>
        ///             <description>64-bit two's complement binary integer</description>
        ///         </item>
        ///         <item>
        ///             <term>-32</term>
        ///             <description>IEEE single precision floating point</description>
        ///         </item>
        ///         <item>
        ///             <term>-64</term>
        ///             <description>IEEE double precision floating point</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <value>
        ///     The bits per pixel. Note that this value can be negative, indicating that IEEE floating point format
        ///     is used.
        /// </value>
        /// <remarks>
        ///     The total number of bits in the primary data array, exclusive of fill that is needed after the data to
        ///     complete the last 2880-byte data block, is given by:
        ///     <c>
        ///         Nbits = |BITPIX| × (NAXIS1 × NAXIS2 × · · · × NAXISm)
        ///     </c>
        /// </remarks>
        [FitsKeyword("BITPIX")]
        public int BitsPerPixel { get; set; }
        }
    }