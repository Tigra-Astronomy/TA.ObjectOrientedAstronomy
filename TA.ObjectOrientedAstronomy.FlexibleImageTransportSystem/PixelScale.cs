// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: PixelScale.cs  Last modified: 2016-11-07@20:23 by Tim Long

using JetBrains.Annotations;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem
    {
    internal class PixelScale
        {
        [FitsKeyword("BZERO")]
        [UsedImplicitly]
        public double ZeroOffset { get; set; }

        [FitsKeyword("BSCALE")]
        [UsedImplicitly]
        public double Scale { get; set; } = 1.0;
        }
    }