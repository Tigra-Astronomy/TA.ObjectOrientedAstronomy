using System.Runtime.InteropServices.ComTypes;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FundamentalTypes;
using TA.ObjectOrientedAstronomy.Observatory;
using TA.ObjectOrientedAstronomy.Specifications.Observatory;

namespace TA.Dpoint.Specifications
    {
    [Subject(typeof(WallaceDomeSync), "smoke test")]
    internal class when_computing_dome_position_for_wallace_test_case
        {
        private static DomePosition Expected = new DomePosition(50.369411, 72.051742);
        private static DomePosition Actual;
        private static ObservatoryGeometry geometry;
        static Declination MechanicalDeclination = new Declination(37.9);
        static HourAngle MechanicalHourAngle = new HourAngle(0, 10, 0);
        Establish context = () => geometry = new ObservatoryGeometry()
            {
            MountOffsetEast = -35.0,
            MountOffsetNorth = 370,
            MountOffsetUp = 1250,
            DeclinationOpticalAxisDistance = 0,
            DomeRadius = 1900,
            ObservatoryLatitude = new Latitude(36.18),
            PolarDeclinationAxisDistance = 0,
            PolarOpticalAxisDistance = 505
            };
        Because of = () =>
            Actual = new WallaceDomeSync(geometry).FromTelescopeMechanicalPosition(MechanicalHourAngle,
                MechanicalDeclination);
        It should_compute_the_wallace_dome_position = () => Actual.ShouldEqual(Expected, 0.005);
        }

    /*
     * Test the case for an equatorial fork mount carefully positioned at the centre of the dome
     * so that all offsets are zero. In this case, the dome azimuth should equal HA+180.
     * The dome elevation should equal (90 - Latitude),
     * i.e. on the intersection of the celestial equator and local meridian.
     */
    [Subject(typeof(WallaceDomeSync), "null hypothesis")]
    internal class when_computing_dome_position_with_telescope_at_centre_of_dome_using_wallace
        {
        private static readonly double Latitude = 36.18;
        private static DomePosition Expected = new DomePosition(180.0, 90.0-Latitude);
        private static DomePosition Actual;
        private static ObservatoryGeometry geometry;
        static Declination MechanicalDeclination = new Declination(0);
        static HourAngle MechanicalHourAngle = new HourAngle(0, 0, 0);
        Establish context = () => geometry = new ObservatoryGeometry()
            {
            MountOffsetEast = 0,
            MountOffsetNorth = 0,
            MountOffsetUp = 0,
            DeclinationOpticalAxisDistance = 0,
            DomeRadius = 1900,
            ObservatoryLatitude = new Latitude(Latitude),
            PolarDeclinationAxisDistance = 0,
            PolarOpticalAxisDistance = 0
            };
        Because of = () =>
            Actual = new WallaceDomeSync(geometry).FromTelescopeMechanicalPosition(MechanicalHourAngle,
                MechanicalDeclination);
        It should_compute_the_wallace_dome_position = () => Actual.ShouldEqual(Expected, 0.005);
        }


    [Subject(typeof(UnitConversionExtensions), "Hour Angle")]
    internal class when_converting_hours_to_degrees
        {
        It should_multiply_by_15 = () => new HourAngle(1).InDegrees().ShouldEqual(15.0);
        }

    [Subject(typeof(UnitConversionExtensions), "Hour Angle")]
    internal class when_converting_hour_angle_to_radians
        {
        It should_agree_with_wallace = () => new HourAngle(0, 10, 0).InRadians().ShouldBeCloseTo(0.0436, 0.0001);
        }

    [Subject(typeof(UnitConversionExtensions), "Declination")]
    internal class when_converting_declination_to_radians
        {
        It should_agree_with_wallace = () => new Declination(37.9).InRadians().ShouldBeCloseTo(0.6615, 0.0001);
        }

    [Subject(typeof(UnitConversionExtensions), "Latitude")]
    internal class when_converting_latitude_to_radians
        {
        It should_agree_with_wallace = () => new Latitude(36.18).InRadians().ShouldBeCloseTo(0.6315, 0.0001);
        }
    }