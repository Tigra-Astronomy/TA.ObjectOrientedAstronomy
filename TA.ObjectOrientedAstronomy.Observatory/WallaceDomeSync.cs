using TA.ObjectOrientedAstronomy.FundamentalTypes;
using static System.Math;

namespace TA.ObjectOrientedAstronomy.Observatory
    {
    /// <summary>
    /// Calculates the optimum dome position (azimuth and elevation) for a given mount geometry and telescope position.
    /// </summary>
    /// <remarks>
    /// See "Dome Predictions for an Equatorial Telescope"  at http://www.tpointsw.uk/edome.pdf
    /// by Patrick T. Wallace, Tpoint Consulting {tpw@tpointsw.uk} [Wallace].
    /// </remarks>
    public class WallaceDomeSync
        {
        private readonly ObservatoryGeometry geometry;

        public WallaceDomeSync(ObservatoryGeometry geometry)
            {
            this.geometry = geometry;
            }

        public DomePosition FromTelescopeMechanicalPosition(HourAngle mechanicalHourAngle, Declination mechanicalDeclination)
            {
            // Unpack the various inputs so that we have names matching the equations in [Wallace].
            var h = mechanicalHourAngle.InRadians();
            var δ = mechanicalDeclination.InRadians();
            var p = geometry.PolarDeclinationAxisDistance;
            var q = geometry.PolarOpticalAxisDistance;
            var r = geometry.DeclinationOpticalAxisDistance;
            var φ = geometry.ObservatoryLatitude.InRadians();
            var rD = geometry.DomeRadius;
            var xm = geometry.MountOffsetEast;
            var ym = geometry.MountOffsetNorth;
            var zm = geometry.MountOffsetUp;

            // Numbers in braces () in the comments refer to equations in [Wallace].
            // Calculate vector mount to optical center
            var y = p + r * Sin(δ);             // (1)
            var xmo = q * Cos(h) + y * Sin(h);  // (2)
            var ymo = -q * Sin(h) + y * Cos(h); // (3)
            var zmo = r * Cos(δ);               // (4)

            // Calculate vector dome to optical center in east-north-up frame
            var xdo = xm + xmo;                         // (5)
            var ydo = ym + ymo * Sin(φ) + zmo * Cos(φ); // (6)
            var zdo = zm - ymo * Cos(φ) + zmo * Sin(φ); // (7)

            // Calculate the telescope (A,E) unit vector in the east-north-up frame
            var x1 = -Sin(h) * Cos(δ);              // (8)
            var y1 = -Cos(h) * Cos(δ);              // (9)
            var z1 = Sin(δ);                        // (10)
            var xs = x1;                            // (11)
            var ys = y1 * Sin(φ) + z1 * Cos(φ);     // (12)
            var zs = -y1 * Cos(φ) + z1 * Sin(φ);    // (13)

            // Solve for the distance from the optical centre to the dome aperture
            var sdt = xs * xdo + ys * ydo + zs * zdo;       // (14)
            var t2m = xdo * xdo + ydo * ydo + zdo * zdo;    // (15)
            var w = sdt * sdt - t2m + rD * rD;              // (16)
            // ToDo: if w is -ve there is no solution.
            var f = -sdt + Sqrt(w);                         // (17)

            // Calculate vector dome centre to dome aperture
            var xda = xdo + f * xs; // (18)
            var yda = ydo + f * ys; // (19)
            var zda = zdo + f * zs; // (20)

            // Convert to spherical coordinates (in radians)
            var A = Atan2(xda, yda);                            // (21)
            var E = Atan2(zda, Sqrt(xda * xda + yda * yda));    // (22)

            // Package and return the result.
            var domePosition = DomePosition.FromRadians(A, E);
            return domePosition;
            }
        }
    }