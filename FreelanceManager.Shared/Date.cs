using System;

namespace FreelanceManager
{
    public class Date : IEquatable<Date>,
                        IComparable<Date>,
                        IComparable
    {
        public Date(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
            Numeric = new DateTime(year, month, day).GetNumericValue();
        }

        public int Year
        {
            get;
            private set;
        }

        public int Month
        {
            get;
            private set;
        }

        public int Day
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
            get { return string.Format("{0:D4}-{1:D2}-{2:D2}", Year, Month, Day); }
        }

        public static Date Parse(string value)
        {
            var dateDate = DateTime.Parse(value);
            return new Date(dateDate.Year, dateDate.Month, dateDate.Day);
        }

        public override string ToString()
        {
            return Display;
        }

        public static bool operator ==(Date left, Date right)
        {
            if ((object)left == null && (object)right == null)
                return true;

            if ((object)left == null || (object)right == null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Date left, Date right)
        {
            return !(left == right);
        }

        public static bool operator >(Date left, Date right)
        {
            if (left == null && right == null)
                return false;

            if (left == null)
                return false;

            if (right == null)
                return true;

            return left.CompareTo(right) > 0;
        }

        public static bool operator <(Date left, Date right)
        {
            if (left == null && right == null)
                return false;

            if (left == null)
                return true;

            if (right == null)
                return false;

            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(Date left, Date right)
        {
            if (left == null && right == null)
                return true;

            if (left == null)
                return false;

            if (right == null)
                return true;

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(Date left, Date right)
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
            if (!(obj is Date))
            {
                return false;
            }

            var other = (Date)obj;
            return Equals(other);
        }

        #region Implementation of IEquatable<Date>

        public bool Equals(Date other)
        {
            if (other == null)
                return false;

            return Numeric == other.Numeric;
        }

        #endregion

        #region Implementation of IComparable<Date>

        public int CompareTo(Date other)
        {
            return Numeric.CompareTo(other.Numeric);
        }

        #endregion

        #region Implementation of IComparable

        int IComparable.CompareTo(object obj)
        {
            if (obj is Date)
            {
                return CompareTo((Date)obj);
            }

            throw new InvalidOperationException("Object is not a " + GetType() + " instance.");
        }

        #endregion
    }
}
