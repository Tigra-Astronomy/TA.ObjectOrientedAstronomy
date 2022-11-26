using System;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Observatory
    {
    /// <summary>
    ///     Describes an observatory's optical geometry for the purposes of calculating telescope-dome synchronization.
    ///     Units of distance are not specified, but all must be in the same units.
    /// </summary>
    public class ObservatoryGeometry
        {
        /// <summary>
        /// The distance east-west from the dome centre to the mount centre.
        /// A positive value indicates that the mount centre is east of the dome centre.
        /// </summary>
        /// <remarks>Referred to as <c>Xm</c> in [Wallace]</remarks>
        public double MountOffsetEast { get; internal set; }
        /// <summary>
        /// The distance north-south from the dome centre to the mount centre.
        /// A positive value indicates that the mount centre is north of the dome centre.
        /// </summary>
        /// <remarks>Referred to as <c>Ym</c> in [Wallace].</remarks>
        public double MountOffsetNorth { get; internal set; }
        /// <summary>
        /// The distance up-down from the dome centre to the mount centre.
        /// A positive value indicates that the mount centre is above the dome centre.
        /// </summary>
        /// <remarks>Referred to as <c>Zm</c> in [Wallace].</remarks>
        public double MountOffsetUp { get; internal set; }

        /// <summary>
        /// The radius of the dome.
        /// </summary>
        /// <value>
        /// Units are not specified but must be consistent with
        /// <see cref="PolarDeclinationAxisDistance"/>, <see cref="DeclinationOpticalAxisDistance"/>
        /// and <see cref="PolarOpticalAxisDistance"/>.
        /// </value>
        /// <remarks>Referred to a <c>Rd</c> in [Wallace].</remarks>
        public double DomeRadius { get; internal set; }

        /// <summary>
        /// The geographic latitude of the observatory, which defines the inclination of the north
        /// end (which points towards Polaris) of the polar axis. Note: in the southern hemisphere
        /// the north end of the polar axis points below the horizon and will be a negative value.
        /// </summary>
        public Latitude ObservatoryLatitude { get; internal set; }

        /// <summary>
        /// Gets the polar-declination axis separation at closest approach.
        /// This is typically zero because for most mount geometries the two axes intersect,
        /// but this is not always so (for example in the case of some horseshoe mounts).
        /// </summary>
        /// <value>
        /// The value is positive towards the North Celestial Pole when the mount mechanical
        /// Hour Angle = 0 and Declination = 0. Units are not specified but must be consistent with
        /// <see cref="DomeRadius"/>, <see cref="PolarOpticalAxisDistance"/>
        /// and <see cref="DeclinationOpticalAxisDistance"/>
        /// </value>
        public double PolarDeclinationAxisDistance { get; internal set; }

        /// <summary>
        /// The distance along the declination axis from the polar axis to the optical axis.
        /// Typically the declination axis intersects both the polar axis and the optical axis,
        /// so this will be the intersection distance. In cases where the axes do not intersect,
        /// the distance is measured from the point on the declination axis closest to the polar axis,
        /// to the point on the declination axis closest to the optical axis.
        /// </summary>
        /// <value>
        /// Units are not specified but must be consistent with
        /// <see cref="DomeRadius"/>, <see cref="PolarDeclinationAxisDistance"/>
        /// and <see cref="DeclinationOpticalAxisDistance"/>
        /// </value>
        public double PolarOpticalAxisDistance { get; internal set; }

        /// <summary>
        /// Gets the distance from the declination axis to the optical axis.
        /// This is typically zero for most mounts because the optical assembly
        /// is typically centered on the declination axis for balance. However in some cases,
        /// for example when there are multiple instruments on the same mount, the optical
        /// axis could be mounted off to one side.
        /// </summary>
        /// <value>
        /// The value is positive towards the North Celestial Pole when the mount mechanical
        /// Hour Angle = 0 and Declination = 0. Units are not specified but must be consistent with
        /// <see cref="DomeRadius"/>, <see cref="PolarOpticalAxisDistance"/>
        /// and <see cref="PolarDeclinationAxisDistance"/>
        /// </value>
        /// <remarks>This is referred to as <c>r</c> in [Wallace].</remarks>
        public double DeclinationOpticalAxisDistance { get; internal set; }
        }
    }