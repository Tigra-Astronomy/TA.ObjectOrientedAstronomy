// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: DenebRightAscension.cs  Last modified: 2016-10-08@18:43 by Tim Long

using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Specifications
    {
    [Behaviors]
    class DenebRightAscension
        {
        protected static Bearing UUT;
        It should_have_20_hours_ = () => UUT.Degrees.ShouldEqual(20u);
        It should_have_41_minutes = () => UUT.Minutes.ShouldEqual(41u);
        It should_have_59_seconds = () => UUT.Seconds.ShouldEqual(59u);
        }
    }