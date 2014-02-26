using System;
using System.ComponentModel;
using System.Globalization;
using MongoDB.Bson.Serialization.Attributes;

namespace FreelanceManager
{
    public class Money : IEquatable<Money>,
                         IComparable<Money>,
                         IComparable
    {
        public static readonly Money Zero = new Money(0);

        private Money(int inCents)
        {
            InCents = inCents;
        }

        public Money(decimal amount)
        {
            InCents = (int)(Math.Round(amount, 2) * 100);
        }

        public int InCents
        {
            get;
            private set;
        }

        public bool Empty
        {
            get { return InCents == 0; }
        }

        public decimal Value
        {
            get { return (decimal)InCents / 100; }
        }

        public static implicit operator Money(Decimal value)
        {
            return new Money(value);
        }

        public static implicit operator Decimal(Money value)
        {
            if (value == null)
                return 0;

            return value.Value;
        }

        public static Money operator -(Money value)
        {
            return new Money(-value.InCents);
        }

        public static Money operator +(Money value)
        {
            return new Money(+value.InCents);
        }

        public static Money operator +(Money left, Money right)
        {
            return new Money(left.InCents + right.InCents);
        }

        public static Money operator -(Money left, Money right)
        {
            return left + -right;
        }

        public static Money operator *(Money left, Decimal right)
        {
            return ((Decimal)left * right);
        }

        public static Money operator /(Money left, Decimal right)
        {
            return ((Decimal)left / right);
        }

        public static Boolean operator ==(Money left, Money right)
        {
            if ((object)left == null && (object)right == null)
                return true;

            if ((object)left == null || (object)right == null)
                return false;

            return left.Equals(right);
        }

        public static Boolean operator !=(Money left, Money right)
        {
            return !(left == right);
        }

        public static Boolean operator >(Money left, Money right)
        {
            if (left == null && right == null)
                return false;

            if (left == null)
                return false;

            if (right == null)
                return true;

            return left.CompareTo(right) > 0;
        }

        public static Boolean operator <(Money left, Money right)
        {
            if (left == null && right == null)
                return false;

            if (left == null)
                return true;

            if (right == null)
                return false;

            return left.CompareTo(right) < 0;
        }

        public static Boolean operator >=(Money left, Money right)
        {
            if (left == null && right == null)
                return true;

            if (left == null)
                return false;

            if (right == null)
                return true;

            return left.CompareTo(right) >= 0;
        }

        public static Boolean operator <=(Money left, Money right)
        {
            if (left == null && right == null)
                return true;

            if (left == null)
                return true;

            if (right == null)
                return false;

            return left.CompareTo(right) <= 0;
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return InCents.GetHashCode();
            }
        }

        public override Boolean Equals(Object obj)
        {
            if (!(obj is Money))
            {
                return false;
            }

            var other = (Money)obj;
            return Equals(other);
        }

        public override String ToString()
        {
            return Value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        public String ToString(String format)
        {
            return Value.ToString(format);
        }

        #region Implementation of IEquatable<Money>

        public Boolean Equals(Money other)
        {
            if (other == null)
                return false;

            return InCents == other.InCents;
        }

        #endregion

        #region Implementation of IComparable<Money>

        public Int32 CompareTo(Money other)
        {
            return InCents.CompareTo(other.InCents);
        }

        #endregion


        #region Implementation of IComparable

        int IComparable.CompareTo(object obj)
        {
            if (obj is Money)
            {
                return CompareTo((Money)obj);
            }

            throw new InvalidOperationException("Object is not a " + GetType() + " instance.");
        }

        #endregion
    }
}
