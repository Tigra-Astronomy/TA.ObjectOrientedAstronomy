// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015 Tigra Astronomy, all rights reserved.
// 
// File: AstroDateTime.cs  Last modified: 2015-11-21@16:44 by Tim Long

using System;
using System.Diagnostics.Contracts;

namespace TA.ObjectOrientedAstronomy.FundamentalTypes
{
    /// <summary>
    ///     Class AstroDateTime provides conversion functions from a DateTime object (containing a time in UTC) to various
    ///     other formats.
    /// </summary>
    public sealed class AstroDateTime
    {
        /// <summary>
        ///     Julian date corresponding to Noon, 1st Jan 1970 (J1970)
        /// </summary>
        public const double J1970 = 2440587.5;

        /// <summary>
        ///     The number of seconds in a calendar day
        /// </summary>
        public const double SecondsPerDay = 86400.0;

        /// <summary>
        ///     The number of minutes in a calendar day
        /// </summary>
        public const double MinutesPerDay = 1440.0;

        /// <summary>
        ///     The number of hours in a calendar day
        /// </summary>
        public const double HoursPerDay = 24.0;

        /// <summary>
        ///     Julian date corresponding to Noon, 1st Jan 2000 (J2000).
        /// </summary>
        public const double J2000 = 2451545.0;

        /// <summary>
        ///     Site longditude in decimal degrees
        /// </summary>
        readonly double longitude;

        /// <summary>
        ///     Offset (in decimal hours) from Greenwich Mean Siderial Time of the configured longditude
        /// </summary>
        double siteGmstOffsetHours;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AstroDateTime" /> class with its internal
        ///     timing reference configured as the current date and time at the prime meridian
        ///     (longitude 0).
        /// </summary>
        public AstroDateTime()
        {
            UtcDateTime = DateTime.UtcNow;
            longitude = 0.0;
            siteGmstOffsetHours = 0.0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AstroDateTime" /> class with its internal
        ///     time reference set to the current time (expressed as UTC) and with the specified
        ///     geographic longitude.
        /// </summary>
        /// <param name="lnSite">The geographic longitude.</param>
        public AstroDateTime(Longitude lnSite)
        {
        Contract.Requires(lnSite!=null);
            longitude = lnSite.Value;
            siteGmstOffsetHours = longitude / 15.0;
            UtcDateTime = DateTime.UtcNow;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AstroDateTime" /> class with its internal
        ///     time reference set to the specified date, time and longitude.
        /// </summary>
        /// <param name="lnSite">The ln site.</param>
        /// <param name="dtUtc">The dt UTC.</param>
        public AstroDateTime(Longitude lnSite, DateTime dtUtc)
        {
            Contract.Requires(lnSite != null);
            longitude = lnSite.Value;
            siteGmstOffsetHours = longitude / 15.0;
            UtcDateTime = dtUtc;
        }

        /// <summary>
        ///     The date and time in Universal Time Coordinated (UTC)
        /// </summary>
        public DateTime UtcDateTime { get; set; }

        /// <summary>
        ///     Gets the current local siderial time for the configured longitude.
        /// </summary>
        public double LstNow { get { return UtcToLocalSiderialTime(DateTime.UtcNow); } }

        /// <summary>
        ///     The date when the Gregorian calendar was introduced, replacing the Julian calendar (expressed as the Julian date)
        /// </summary>
        public static DateTime JulianGregorianTransition { get { return new DateTime(1582, 10, 4, 0, 0, 0); } }

        /// <summary>
        ///     Takes a DateTime object representing a UTC time and returns
        ///     the number of days since Noon, 1 Jan 4713 BC (Julian Days).
        ///     1 Jan 4713 0.5 is the fundamental epoch.
        /// </summary>
        /// <remarks>
        ///     This code is based in part on the book "Astronomy With Your Personal Computer"
        ///     ISBN 0-521-31976-5, Peter Duffett-Smith
        /// </remarks>
        public static double UtcToJulianDays(DateTime dtUtc)
        {
            // A DateTime structure records the number of 100-nanosecond intervals that
            // have elapsed since 12:00 A.M., January 1, 0001.

            int nYears;
            int nMonths;

            if (dtUtc.Month < 3)
            {
                nYears = dtUtc.Year - 1;
                nMonths = dtUtc.Month + 12;
            }
            else
            {
                nYears = dtUtc.Year;
                nMonths = dtUtc.Month;
            }

            var dA = Math.Floor((double) (nYears / 100)); // Number of centuries

            // Adjust for transition from Julian to Gregorian calendars.
            // Gregorian calendar was introduced 4th October 1582, but Pope
            // Gregory abolished the days 5th to 14th October 1582 inclusive.

            var dB = 0.0;
            if (dtUtc >= JulianGregorianTransition)
            {
                // Gregorian date    
                dB = 2 - dA + Math.Floor(dA / 4);
            }

            /* add a fraction of hours, minutes and secs to days*/
            var dDays = dtUtc.Day + dtUtc.Hour / HoursPerDay + dtUtc.Minute / MinutesPerDay
                        + dtUtc.Second / SecondsPerDay;

            /* now get the JD */
            var dJulian = Math.Floor(365.25 * (nYears + 4716.0)) + Math.Floor(30.6001 * (nMonths + 1)) + dDays + dB
                          - 1524.5;
            return dJulian;
        }

        /// <summary>
        ///     Calculate the Greenwich Mean Siderial Time for the specified Julian Date
        /// </summary>
        public static double JulianDateToGreenwichMeanSiderealTime(double julianDays)
        {
            var dT = (julianDays - J2000) / 36525.0;

            // GMST expressed as an angle
            var sidereal = 280.46061837 + 360.98564736629 * (julianDays - 2451545.0) + 0.000387933 * dT * dT
                           - dT * dT * dT / 38710000.0;

            // Convert the angle to the equivalent positive angle [0..360]
            var b = new Bearing(sidereal);
            sidereal = b.Value;
            sidereal *= 24.0 / 360.0; // Change from degrees to hours.
            return sidereal;
        }

        /// <summary>
        ///     Convert a DateTime object representing a UTC time to Local Siderial Time for the configured longditude.
        /// </summary>
        /// <param name="utc">The UTC date and time to be converted.</param>
        /// <returns></returns>
        public double UtcToLocalSiderialTime(DateTime utc)
        {
            var jd = UtcToJulianDays(utc);
            var gmst = JulianDateToGreenwichMeanSiderealTime(jd);
            // TODO: Account for NUTATION
            var lst = gmst + longitude / 15.0;
            while (lst < 0.0)
            {
                // Guard against getting a -ve LST
                lst += 24.0;
            }
            lst %= 24.0; // One final safety net, take the modulo 24 hours.
            return lst;
        }
    }
}