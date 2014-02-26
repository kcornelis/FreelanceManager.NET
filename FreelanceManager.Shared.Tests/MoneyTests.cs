using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FreelanceManager
{
    public class MoneyTests
    {
        [Fact]
        public void Money_Has_A_Value_Property()
        {
            var money1 = new Money(30.25M);
            var money2 = new Money(30);

            Assert.Equal(money1.Value, 30.25M);
            Assert.Equal(money2.Value, 30M);
        }

        [Fact]
        public void Money_Has_A_InCents_Property()
        {
            var money1 = new Money(30.25M);
            var money2 = new Money(30);

            Assert.Equal(money1.InCents, 3025);
            Assert.Equal(money2.InCents, 3000);
        }

        [Fact]
        public void If_Value_Is_0_Then_Its_Empty()
        {
            var empty = new Money(0);

            Assert.True(empty.Empty);
        }

        [Fact]
        public void If_Value_Is_Not_0_Then_Its_Not_Empty()
        {
            var empty = new Money(1);

            Assert.False(empty.Empty);
        }

        [Fact]
        public void Money_Is_Rounded_To_2_Decimals()
        {
            var money1 = new Money(30.333M);
            var money2 = new Money(30.339M);
            var money3 = new Money(30.335M);

            Assert.Equal(money1.Value, 30.33M);
            Assert.Equal(money2.Value, 30.34M);
            Assert.Equal(money3.Value, 30.34M);
        }

        [Fact]
        public void Money_Can_Cast_To_Decimal()
        {
            var money = new Money(30.25M);

            Assert.Equal(30.25M, (decimal)money);
        }

        [Fact]
        public void Decimal_Can_Cast_To_Money()
        {
            var money = (Money)30.25M;

            Assert.Equal(30.25M, money.Value);
        }

        [Fact]
        public void Money_Can_Cast_To_Decimal_When_Money_Is_Null()
        {
            var money = (Money)null;

            Assert.Equal(0, (decimal)money);
        }

        [Fact]
        public void Money_Can_Be_Negative()
        {
            var money = new Money(30);

            Assert.Equal(-30, -money);
        }

        [Fact]
        public void Money_Can_Be_Added_With_Another_Money()
        {
            var money1 = new Money(30);
            var money2 = new Money(50);

            Assert.Equal(80, money1 + money2);
        }

        [Fact]
        public void Money_Can_Be_Substracted_With_Another_Money()
        {
            var money1 = new Money(30);
            var money2 = new Money(50);

            Assert.Equal(-20, money1 - money2);
        }

        [Fact]
        public void Money_Can_Be_Multiplied_With_Another_Money()
        {
            var money1 = new Money(30);
            var money2 = new Money(50);

            Assert.Equal(1500, money1 * money2);
        }

        [Fact]
        public void Money_Can_Be_Divided_With_Another_Money()
        {
            var money1 = new Money(50);
            var money2 = new Money(10);

            Assert.Equal(5, money1 / money2);
        }

        [Fact]
        public void Money_With_The_Same_Value_Should_Be_Equal()
        {
            var money1 = new Money(50);
            var money2 = new Money(50);

            Assert.Equal(money1, money2);
            Assert.True(money1 == money2);
            Assert.False(money1 != money2);
            Assert.True(money1.Equals(money2));
        }

        [Fact]
        public void Money_With_Another_Value_Should_Not_Be_Equal()
        {
            var money1 = new Money(50);
            var money2 = new Money(50.01M);
            var money3 = new Money(-50);

            Assert.NotEqual(money1, money2);
            Assert.NotEqual(money1, money3);

            Assert.False(money1 == money2);
            Assert.True(money1 != money2);
            Assert.False(money1.Equals(money2));
        }

        [Fact]
        public void Money_Compare_LargerThen()
        {
            var money1 = new Money(50);
            var money2 = new Money(50);
            var moneyLarger = new Money(50.01M);

            Assert.True(moneyLarger > money1);
            Assert.False(money1 > moneyLarger);
            Assert.False(money1 > money2);
        }

        [Fact]
        public void Money_Compare_SmallerThen()
        {
            var money1 = new Money(50);
            var money2 = new Money(50);
            var moneyLarger = new Money(50.01M);

            Assert.False(moneyLarger < money1);
            Assert.False(money2 < money1);
            Assert.True(money1 < moneyLarger);
        }

        [Fact]
        public void Money_Compare_LargerThenOrEqual()
        {
            var money1 = new Money(50);
            var money2 = new Money(50);
            var moneyLarger = new Money(50.01M);

            Assert.True(moneyLarger >= money1);
            Assert.False(money1 >= moneyLarger);
            Assert.True(money1 >= money2);
        }

        [Fact]
        public void Money_Compare_SmallerThenOrEqual()
        {
            var money1 = new Money(50);
            var money2 = new Money(50);
            var moneyLarger = new Money(50.01M);

            Assert.False(moneyLarger <= money1);
            Assert.True(money2 <= money1);
            Assert.True(money1 <= moneyLarger);
        }

        [Fact]
        public void Money_ToString()
        {
            var money1 = new Money(50);
            var money2 = new Money(50.01M);
            var money3 = new Money(-50.01M);

            Assert.Equal("50.00", money1.ToString());
            Assert.Equal("50.01", money2.ToString());
            Assert.Equal("-50.01", money3.ToString());
        }
    }
}
