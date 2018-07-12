// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: HistogramStretch.cs  Last modified: 2016-11-07@20:23 by Tim Long

using JetBrains.Annotations;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Histogram stretching information used when converting raw image data to a display bitmap.
    /// </summary>
    public class HistogramStretch
        {
        /// <summary>
        ///     Gets or sets the black point, or lowest allowed data value. All data values lower than the black point
        ///     are treated as if they are equal to the black point.
        /// </summary>
        /// <value>The black point, in Analogue-to-Digital Units (ADU).</value>
        [FitsKeyword("CBLACK")]
        [UsedImplicitly]
        public double BlackPoint { get; set; }

        /// <summary>
        ///     Gets or sets the white point, or highest allowed data value. All data values higher than the white point
        ///     are treated as if they are equal to the white point.
        /// </summary>
        /// <value>The white point, in Analogue-to-Digital Units (ADU).</value>
        [FitsKeyword("CWHITE")]
        [UsedImplicitly]
        public double WhitePoint { get; set; } = short.MaxValue;

        /// <summary>
        ///     Gets or sets the histogram stretching strategy.
        /// </summary>
        public HistogramShape StretchMethod { get; set; } = HistogramShape.Linear;
        }
    }