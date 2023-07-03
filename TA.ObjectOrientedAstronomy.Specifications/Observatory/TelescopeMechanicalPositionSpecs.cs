// This file is part of the TA.Dpoint project
// 
// Copyright © 2015-2021 Tigra Astronomy, all rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so. The Software comes with no warranty of any kind.
// You make use of the Software entirely at your own risk and assume all liability arising from your use thereof.
// 
// File: TelescopeMechanicalPositionSpecs.cs  Last modified: 2021-02-22@02:49 by Tim Long

using System;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.Observatory;
using TA.ObjectOrientedAstronomy.Specifications.TestHelpers;

namespace TA.ObjectOrientedAstronomy.Specifications.Observatory
    {
    [Subject(typeof(TelescopeMechanicalPosition), "Validation")]
    internal class when_creating_from_an_invalid_position
        {
        It should_throw_if_ha_less_than_zero = () => Should.Throw<ArgumentOutOfRangeException>(()=>new TelescopeMechanicalPosition(0-Double.Epsilon,0));
        It should_throw_if_declination_below_n90 = () => Should.Throw<ArgumentOutOfRangeException>(()=>new TelescopeMechanicalPosition(0,-90.00000001));
        It should_throw_if_ha_too_big = () => Should.Throw<ArgumentOutOfRangeException>(()=>new TelescopeMechanicalPosition(24.0,0));
        It should_throw_if_declination_above_90 = () => Should.Throw<ArgumentOutOfRangeException>(()=>new TelescopeMechanicalPosition(0,90.00000001));
        }
    }