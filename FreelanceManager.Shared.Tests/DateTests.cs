using Xunit;

namespace FreelanceManager
{
    public class DateTests
    {
        [Fact]
        public void Parse_Should_Parse_Dates_1()
        {
            var date = Date.Parse("2014-01-02");

            Assert.Equal(2014, date.Year);
            Assert.Equal(1, date.Month);
            Assert.Equal(2, date.Day);
        }

        [Fact]
        public void Parse_Should_Parse_Dates_2()
        {
            var date = Date.Parse("2014-1-2");

            Assert.Equal(2014, date.Year);
            Assert.Equal(1, date.Month);
            Assert.Equal(2, date.Day);
        }

        [Fact]
        public void ToString_Should_Display_The_Date_1()
        {
            var date = Date.Parse("2014-01-02");

            Assert.Equal("2014-01-02", date.ToString());
        }

        [Fact]
        public void ToString_Should_Display_The_Date_2()
        {
            var date = Date.Parse("2014-1-2");

            Assert.Equal("2014-01-02", date.ToString());
        }

        [Fact]
        public void Numeric_Test()
        {
            var date = Date.Parse("2014-1-2");

            Assert.Equal(20140102, date.Numeric);
        }

        [Fact]
        public void Dates_With_The_Same_Date_Should_Be_Equal()
        {
            var date1 = Date.Parse("2014-1-2");
            var date2 = Date.Parse("2014-1-2");

            Assert.Equal(date1, date2);
            Assert.True(date1 == date2);
            Assert.False(date1 != date2);
            Assert.True(date1.Equals(date2));
        }

        [Fact]
        public void Dates_With_Another_Date_Should_Not_Be_Equal()
        {
            var date1 = Date.Parse("2014-1-2");
            var date2 = Date.Parse("2014-1-3");
            var date3 = Date.Parse("2014-2-2");
            var date4 = Date.Parse("2015-1-2");

            Assert.NotEqual(date1, date2);
            Assert.NotEqual(date1, date3);
            Assert.NotEqual(date1, date4);

            Assert.False(date1 == date2);
            Assert.True(date1 != date2);
            Assert.False(date1.Equals(date2));
        }

        [Fact]
        public void Dates_Compare_LargerThen()
        {
            var date1 = Date.Parse("2014-1-2");
            var date2 = Date.Parse("2014-1-2");
            var dateDayLater = Date.Parse("2014-1-3");
            var dateMonthLater = Date.Parse("2014-2-3");

            Assert.True(dateDayLater > date1);
            Assert.True(dateMonthLater > date1);
            Assert.False(date1 > dateDayLater);
            Assert.False(date1 > date2);
        }

        [Fact]
        public void Dates_Compare_SmallerThen()
        {
            var date1 = Date.Parse("2014-1-2");
            var date2 = Date.Parse("2014-1-2");
            var dateDayLater = Date.Parse("2014-1-3");
            var dateMonthLater = Date.Parse("2014-2-3");

            Assert.False(dateDayLater < date1);
            Assert.False(date2 < date1);
            Assert.True(date1 < dateDayLater);
            Assert.True(date1 < dateMonthLater);
        }

        [Fact]
        public void Dates_Compare_LargerThenOrEqual()
        {
            var date1 = Date.Parse("2014-1-2");
            var date2 = Date.Parse("2014-1-2");
            var dateDayLater = Date.Parse("2014-1-3");
            var dateMonthLater = Date.Parse("2014-2-3");

            Assert.True(dateDayLater >= date1);
            Assert.True(dateMonthLater >= date1);
            Assert.False(date1 >= dateDayLater);
            Assert.True(date1 >= date2);
        }

        [Fact]
        public void Dates_Compare_SmallerThenOrEqual()
        {
            var date1 = Date.Parse("2014-1-2");
            var date2 = Date.Parse("2014-1-2");
            var dateDayLater = Date.Parse("2014-1-3");
            var dateMonthLater = Date.Parse("2014-2-3");

            Assert.False(dateDayLater <= date1);
            Assert.True(date2 <= date1);
            Assert.True(date1 <= dateDayLater);
            Assert.True(date1 <= dateMonthLater);
        }
    }
}
