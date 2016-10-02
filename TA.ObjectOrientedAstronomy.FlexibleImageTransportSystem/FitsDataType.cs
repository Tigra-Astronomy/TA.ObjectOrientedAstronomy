// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: FitsDataType.cs  Last modified: 2016-10-02@06:27 by Tim Long

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     The types of data that can be associated with a Header Data Unit (HDU).
    /// </summary>
    public enum FitsDataType
        {
        /// <summary>
        ///     Indicates that there is no data associated with an HDU
        /// </summary>
        None,
        /// <summary>
        ///     The HDU contains a bitmapped image
        /// </summary>
        Image,
        /// <summary>
        ///     The HDU contains a Table
        /// </summary>
        Table,
        /// <summary>
        ///     The HDU contains a Binary Table
        /// </summary>
        BinaryTable,
        /// <summary>
        ///     The HDU contains a nonstandard/custom data extension.
        /// </summary>
        Other = 99
        }
    }