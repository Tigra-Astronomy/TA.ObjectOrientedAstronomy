// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsHeaderDataUnit.cs  Last modified: 2016-10-02@08:31 by Tim Long

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Represents a single complete Header-Data-Unit (either the Primary HDU or an Extension HDU) including the
    ///     header records and the data array if one is present.
    /// </summary>
    public class FitsHeaderDataUnit
        {
        public FitsHeader Header { get; set; }

        /// <summary>
        ///     Gets or sets an object containing values parsed from the mandatory keywords.
        /// </summary>
        /// <value>
        ///     An instance of <see cref="FitsMandatoryKeywords" /> (or derived type
        ///     <see cref="FitsPrimaryHduMandatoryKeywords" />) with properties set to the values parsed from corresponding
        ///     FITS header records.
        /// </value>
        public FitsMandatoryKeywords MandatoryKeywords { get; set; }

        /// <summary>
        ///     Gets or sets the type of the data associated with this HDU.
        /// </summary>
        /// <value>The type of the data.</value>
        public FitsDataType DataType { get; set; }

        /// <summary>
        ///     Gets or sets the raw FITS data.
        ///     The size of the array returned is given by
        ///     <code lang="text">
        ///         Nbits = |BITPIX| × (NAXIS1 × NAXIS2 × · · · × NAXISm)
        ///         Nbytes = Nbits / 8
        ///     </code>
        /// </summary>
        /// <value>The raw data.</value>
        public byte[] RawData { get; set; }

        /// <summary>
        ///     Gets or sets the data array length bits.
        /// </summary>
        /// <value>The data array length bits.</value>
        public int DataArrayLengthBits { get; set; }

        public int DataArrayLengthBytes => DataArrayLengthBits / 8;
        }
    }