// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: SbFitsExt.cs  Last modified: 2020-12-12@12:32 by Tim Long

using System;
using System.Collections.Generic;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Static methods for creating SIMPLE FITS images that conform to the SBFITSEXT 1.0 Standard. See
    ///     https://diffractionlimited.com/wp-content/uploads/2016/11/sbfitsext_1r0.pdf
    /// </summary>
    public static class SbFitsExt
        {
        public static FitsHeaderDataUnit CreateStandardSimpleFits(SbFitsExtHeader header, byte[] data)
            {
            var primaryHdu = new FitsHeaderDataUnit();
            primaryHdu.MandatoryKeywords = new FitsMandatoryKeywords
                {
                BitsPerPixel = header.BitsPerPixel,
                NumberOfAxes = header.NumberOfAxes,
                LengthOfAxis = header.LengthOfAxis
                };
            throw new NotImplementedException("ToDo");
            }
        }

    public class SbFitsExtHeader : FitsPrimaryHduMandatoryKeywords
        {
        public SbFitsExtHeader()
            {
            Simple = true;
            NumberOfAxes = 2;
            }

        /// <summary>
        ///     Gets or sets the name or catalogue number of the target under observation. Examples: "Crab
        ///     Nebula", "M1", "Messier 1".
        /// </summary>
        [FitsKeyword("OBJECT")]
        public string TargetName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the telescope used for the observation. Examples: "Meade LX-200",
        ///     "PlaneWave CDK-17", "Hubble Space Telescope".
        /// </summary>
        [FitsKeyword("TELESCOP")]
        public string TelescopeName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the camera or measuring instrument used for the observation. Examples:
        ///     "STXL-6303E", "WFPC2"
        /// </summary>
        [FitsKeyword("INSTRUME")]
        public string CameraName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the observer. Usually the name of an individual. Example: "Tim
        ///     Long"
        /// </summary>
        [FitsKeyword("OBSERVER")]
        public string ObserverName { get; set; }

        /// <summary>Gets or sets the UTC date and time of the start of the exposure or observation.</summary>
        [FitsKeyword("DATE-OBS")]
        public DateTime ExposureStartDateTime { get; set; }

        /// <summary>Gets or sets the scale of the data values. Typically 1.0.</summary>
        /// <seealso cref="ZeroOffset" />
        /// <remarks>
        ///     Setting this property does not change data values. It is the responsibility of the user to
        ///     correctly interpret data values.
        /// </remarks>

        [FitsKeyword("BSCALE")]
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        ///     Gets or sets the zero offset of the data values. Typically set to 32768.0 to shift positive
        ///     data into the range of 16-bit signed integers.
        /// </summary>
        /// <seealso cref="Scale" />
        /// <remarks>
        ///     Setting this property does not change data values. It is the responsibility of the user to
        ///     correctly interpret data values.
        /// </remarks>
        [FitsKeyword("BZERO")]
        public float ZeroOffset { get; set; } = 32768.0f;

        /// <summary>
        /// Gets or sets the history of the FITS file.
        /// </summary>
        [FitsKeyword("HISTORY")]
        public List<string> History { get; set; } = new List<string>
                {"Created with TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem"};
        }
    }