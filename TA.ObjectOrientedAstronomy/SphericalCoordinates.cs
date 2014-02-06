namespace TA.ObjectOrientedAstronomy
    {
    /// <summary>
    /// Struct SphericalCoordinates - represents a position expressed as a spherical coordinate
    /// containing latitude, longitude and radius.
    /// </summary>
    public struct SphericalCoordinates
        {
        /// <summary>
        /// Initializes a new instance of the <see cref="SphericalCoordinates"/> struct.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="radius">The radius.</param>
        public SphericalCoordinates(double latitude, double longitude, double radius) : this()
            {
            Latitude = latitude;
            Longitude = longitude;
            Radius = radius;
            }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        /// <value>The latitude, in radians.</value>
        public double Latitude { get; private set; }
        /// <summary>
        /// Gets the longitude.
        /// </summary>
        /// <value>The longitude, in radians.</value>
        public double Longitude { get; private set; }
        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>The radius, in Astronomical Units (AU).</value>
        public double Radius { get; private set; }
        }
    }