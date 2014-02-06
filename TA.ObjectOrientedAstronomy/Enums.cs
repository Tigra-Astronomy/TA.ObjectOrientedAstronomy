namespace TA.ObjectOrientedAstronomy
{
    /// <summary>
    /// Enum SolarSystemBody - enumerates the solar system bodies encompassed by VSOP87
    /// </summary>
public enum SolarSystemBody
    {
    Sun = 0,
    Mercury = 1,
    Venus = 2,
    Earth = 3,
    Mars = 4,
    Jupiter=5,
    Saturn=6,
    Uranus=7,
    Neptune=8,
    EarthMoonBarycentre = 9
    }

/// <summary>
/// Enum CoordinateSystem - represents a coordinate system that can be produced by VSOP87
/// </summary>
public enum CoordinateSystem
    {
    HeliocentricEllipticElements,
    HeliocentricRectangularCoordinates,
    HeliocentricSphericalCoordinates,
    BarycentricRectangularCoordinates,
    }

/// <summary>
/// Enum ReferenceFrame - represents a reference frame in time against which the coordinates produced by VSOP87
/// are valid. For different times, precession must be used.
/// </summary>
public enum ReferenceFrame
    {
    EquinoxJ2000,
    EquinoxJNow
    }

}
