// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsPrimaryHduMandatoryKeywords.cs  Last modified: 2016-10-02@06:41 by Tim Long

using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    public class FitsPrimaryHduMandatoryKeywords : FitsMandatoryKeywords
        {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="FitsPrimaryHduMandatoryKeywords" /> contains a
        ///     valid SIMPLE header. All conforming FITS files set this value to <c>true</c>. If this value is
        ///     <c>false</c> then the results of operations on the file are undefined and we recommend treating the file
        ///     with suspicion.
        /// </summary>
        /// <value><c>true</c> for all conforming FITS files; otherwise, <c>false</c>.</value>
        [FitsKeyword("SIMPLE", Sequence = 1)]
        public bool Simple { get; set; } = true;
        }
    }