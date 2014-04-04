using System;

namespace FreelanceManager
{
    public class Time : IEquatable<Time>,
                        IComparable<Time>,
                        IComparable
    {
        public Time(int hour, int minutes)
        {
            Hour = hour;
            Minutes = minutes;
            Numeric = CalculateNumericValue();
        }

        public int Hour
        {
            get;
            private set;
        }

        public int Minutes
        {
            get;
            private set;
        }

        public int Numeric
        {
            get;
            private set;
        }

        public string Display
        {
            get
            {
                return string.Format("{0:D2}:{1:D2}", Hour, Minutes);
            }
        }

        public int TotalMinutes(Time to)
        {
            if (to == null)
                return 0;

            var difference = (int)(new TimeSpan(to.Hour, to.Minutes, 0) - new TimeSpan(Hour, Minutes, 0)).TotalMinutes;

            if (difference < 0)
                difference = ((60 * 24) + difference);

            return difference;
        }

        private int CalculateNumericValue()
        {
            return Hour * 60 +
                   Minutes;
        }

        public static Time Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            try
            {
                if (value == "24:00")
                    return new Time(0, 0);

                var timespan = TimeSpan.Parse(value);
                return new Time(timespan.Hours, timespan.Minutes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return Display;
        }

        public static bool operator ==(Time left, Time right)
        {
            if ((object)left == null && (object)right == null)
                return true;

            if ((object)left == null || (object)right == null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Time left, Time right)
        {
            return !(left == right);
        }

        public static bool operator >(Time left, Time right)
        {
            if (left == null && right == null)
                return false;

            if (left == null)
                return false;

            if (right == null)
                return true;

            return left.CompareTo(right) > 0;
        }

        public static bool operator <(Time left, Time right)
        {
            if (left == null && right == null)
                return false;

            if (left == null)
                return true;

            if (right == null)
                return false;

            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(Time left, Time right)
        {
            if (left == null && right == null)
                return true;

            if (left == null)
                return false;

            if (right == null)
                return true;

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(Time left, Time right)
        {
            if (left == null && right == null)
                return true;

            if (left == null)
                return true;

            if (right == null)
                return false;

            return left.CompareTo(right) <= 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Numeric.GetHashCode();
            }
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Time))
            {
                return false;
            }

            var other = (Time)obj;
            return Equals(other);
        }

        #region Implementation of IEquatable<Time>

        public bool Equals(Time other)
        {
            if (other == null)
                return false;

            return Numeric == other.Numeric;
        }

        #endregion

        #region Implementation of IComparable<Time>

        public int CompareTo(Time other)
        {
            return Numeric.CompareTo(other.Numeric);
        }

        #endregion

        #region Implementation of IComparable

        int IComparable.CompareTo(object obj)
        {
            if (obj is Time)
            {
                return CompareTo((Time)obj);
            }

            throw new InvalidOperationException("Object is not a " + GetType() + " instance.");
        }

        #endregion
    }
}