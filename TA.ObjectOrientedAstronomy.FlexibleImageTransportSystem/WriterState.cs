// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2020 Tigra Astronomy, all rights reserved.
// 
// File: WriterState.cs  Last modified: 2020-12-12@10:04 by Tim Long

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    /// <summary>
    ///     Used to track the internal state of the FitsWriter (whether it is writing the primary header or
    ///     data blocks, or a secondary header or data blocks).
    /// </summary>
    internal enum WriterState
        {
        PrimaryHeader,
        PrimaryData,
        SecondaryHeader,
        SecondaryData
        }
    }