// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: DeclinationSpecs.cs  Last modified: 2015-11-21@16:46 by Tim Long

using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Specifications
{
    [Subject(typeof(Declination), "Conversion to sexagesimal")]
    class when_converting_a_negative_double_declination_to_sexagesimal
    {
        Because of = () => Dec = new Declination(expectedValue);
        It should_format_correctly = () => Dec.ToString().ShouldEqual("S 06°13'01\"");
        It should_have_the_correct_value = () => Dec.Value.ShouldBeCloseTo(expectedValue);
        It should_have_positive_degrees = () => Dec.Degrees.ShouldBeGreaterThanOrEqualTo(0);
        It should_have_positive_minutes = () => Dec.Minutes.ShouldBeGreaterThanOrEqualTo(0);
        It should_have_positive_seconds = () => Dec.Seconds.ShouldBeGreaterThanOrEqualTo(0);
        static Declination Dec;
        /// <summary>
        ///     The expected value was sampled from the log file attached to AWR-90 at line 3716 which was triggering a
        ///     formatting bug.
        /// </summary>
        const double expectedValue = -6.21712739926718;
    }
}