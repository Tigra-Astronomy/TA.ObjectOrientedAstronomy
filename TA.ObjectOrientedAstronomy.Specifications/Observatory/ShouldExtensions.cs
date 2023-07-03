using System;
using System.Collections.Generic;
using System.Text;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.Observatory;

namespace TA.ObjectOrientedAstronomy.Specifications.Observatory
{
    public static class ShouldExtensions
    {
    public static void ShouldEqual(this DomePosition actual, DomePosition expected, double within)
        {
        actual.Azimuth.Value.ShouldBeCloseTo(expected.Azimuth.Value, within);
        actual.Elevation.Value.ShouldBeCloseTo(expected.Elevation.Value,within);
        }
    }
}
