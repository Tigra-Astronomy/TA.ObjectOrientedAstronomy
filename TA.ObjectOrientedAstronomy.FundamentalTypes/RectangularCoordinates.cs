namespace TA.ObjectOrientedAstronomy.FundamentalTypes
    {
    /// <summary>
    /// Struct RectangularCoordinates - represents a 3D cartesian (rectangular) coordinate
    /// </summary>
    public struct RectangularCoordinates
        {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public RectangularCoordinates(double x, double y, double z) : this()
            {
            X = x;
            Y = y;
            Z = z;
            }
        }
    }