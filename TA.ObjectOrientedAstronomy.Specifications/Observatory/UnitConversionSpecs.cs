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
// File: UnitConversionSpecs.cs  Last modified: 2021-02-27@15:51 by Tim Long


using Machine.Specifications;
using TA.ObjectOrientedAstronomy.Observatory;

namespace TA.Dpoint.Specifications
    {
    [Subject(typeof(UnitConversionExtensions), "Simple Angular Distance")]
    internal class when_measuring_an_angular_distance_in_ha_only
        {
        static readonly TelescopeMechanicalPosition from = new TelescopeMechanicalPosition(0.0, 0.0);
        static readonly TelescopeMechanicalPosition to = new TelescopeMechanicalPosition(1.0, 0.0);
        Because of = () => measured = @from.AngularDistanceTo(to);
            It should_measure_15_degrees = () => measured.ShouldBeCloseTo(15.0, 0.00000001);
        static double measured;
        }
    [Subject(typeof(UnitConversionExtensions), "Simple Angular Distance")]
    internal class when_measuring_an_angular_distance_in_declination_only
        {
        static readonly TelescopeMechanicalPosition from = new TelescopeMechanicalPosition(0.0, 0.0);
        static readonly TelescopeMechanicalPosition to = new TelescopeMechanicalPosition(0.0, 1.0);
        Because of = () => measured = @from.AngularDistanceTo(to);
            It should_measure_1_degrees = () => measured.ShouldBeCloseTo(1.0, 0.00000001);
        static double measured;
        }
    }