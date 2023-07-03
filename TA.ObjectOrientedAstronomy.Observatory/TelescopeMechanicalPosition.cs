using System;
using System.Collections.Generic;
using TA.ObjectOrientedAstronomy.FundamentalTypes;

namespace TA.ObjectOrientedAstronomy.Observatory
    {
    public class TelescopeMechanicalPosition : IComparable<TelescopeMechanicalPosition>, IComparable
        {
        public HourAngle HourAngle { get; }
        public Declination Declination { get; }
        public TelescopeMechanicalPosition(double hourAngle, double declination)
            {
            if (hourAngle < 0 || hourAngle >= 24.0)
                throw new ArgumentOutOfRangeException(nameof(hourAngle), hourAngle, $"Hour angle outside valid range 0 ≤ x < 24.0" );
            if (declination < -90 || declination > 90)
                throw new ArgumentOutOfRangeException(nameof(declination), declination, $"Declination outside valid range -90 ≤ x ≤ 90" );
            HourAngle = new HourAngle(hourAngle);
            Declination = new Declination(declination);
            }

        /// <inheritdoc />
        public int CompareTo(TelescopeMechanicalPosition other)
            {
            var haCompare = this.HourAngle.Value.CompareTo(other.HourAngle.Value);
            if (haCompare != 0)
                return haCompare;
            return this.Declination.Value.CompareTo(other.Declination.Value);
            }

        /// <inheritdoc />
        public int CompareTo(object obj)
            {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is TelescopeMechanicalPosition other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TelescopeMechanicalPosition)}");
            }

        public static bool operator <(TelescopeMechanicalPosition left, TelescopeMechanicalPosition right)
            {
            return Comparer<TelescopeMechanicalPosition>.Default.Compare(left, right) < 0;
            }

        public static bool operator >(TelescopeMechanicalPosition left, TelescopeMechanicalPosition right)
            {
            return Comparer<TelescopeMechanicalPosition>.Default.Compare(left, right) > 0;
            }

        public static bool operator <=(TelescopeMechanicalPosition left, TelescopeMechanicalPosition right)
            {
            return Comparer<TelescopeMechanicalPosition>.Default.Compare(left, right) <= 0;
            }

        public static bool operator >=(TelescopeMechanicalPosition left, TelescopeMechanicalPosition right)
            {
            return Comparer<TelescopeMechanicalPosition>.Default.Compare(left, right) >= 0;
            }
        }
    }