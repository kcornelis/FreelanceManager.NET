using Xunit;

namespace FreelanceManager
{
    public class TimeTests
    {
        [Fact]
        public void Parse_Should_Parse_Times_1()
        {
            var time = Time.Parse("01:20");

            Assert.Equal(1, time.Hour);
            Assert.Equal(20, time.Minutes);
        }

        [Fact]
        public void Parse_Should_Parse_Times_2()
        {
            var time = Time.Parse("2:25");

            Assert.Equal(2, time.Hour);
            Assert.Equal(25, time.Minutes);
        }

        [Fact]
        public void ToString_Should_Display_The_Time_1()
        {
            var time = Time.Parse("01:20");

            Assert.Equal("01:20", time.ToString());
        }

        [Fact]
        public void ToString_Should_Display_The_Time_2()
        {
            var time = Time.Parse("1:20");

            Assert.Equal("01:20", time.ToString());
        }

        [Fact]
        public void TotalMinutes_With_To_Smaller_Then_From()
        {
            var from = Time.Parse("02:00");
            var to = Time.Parse("01:00");

            // 23H
            Assert.Equal(1380, from.TotalMinutes(to));
        }

        [Fact]
        public void TotalMinutes_Gives_The_Time_Difference_In_Minutes()
        {
            var from = new Time(1, 30);
            var to1 = new Time(1, 45);
            var to2 = new Time(18, 45);

            Assert.Equal(15, from.TotalMinutes(to1));
            Assert.Equal(1035, from.TotalMinutes(to2));
        }

        [Fact]
        public void BUGParse_Should_Parse_2400_And_Get_Correct_Total_Minutes()
        {
            var to1 = new Time(23, 00);
            var to2 = new Time(24, 00);
            var to3 = Time.Parse("24:00");

            Assert.Equal(60, to1.TotalMinutes(to2));
            Assert.Equal(60, to1.TotalMinutes(to3));
        }

        [Fact]
        public void Times_With_The_Same_Time_Should_Be_Equal()
        {
            var time1 = new Time(1, 30);
            var time2 = new Time(1, 30);

            Assert.Equal(time1, time2);
            Assert.True(time1 == time2);
            Assert.False(time1 != time2);
            Assert.True(time1.Equals(time2));
        }

        [Fact]
        public void Times_With_Another_Time_Should_Not_Be_Equal()
        {
            var time1 = new Time(1, 30);
            var time2 = new Time(1, 31);
            var time3 = new Time(2, 30);

            Assert.NotEqual(time1, time2);
            Assert.NotEqual(time1, time3);

            Assert.False(time1 == time2);
            Assert.True(time1 != time2);
            Assert.False(time1.Equals(time2));
        }

        [Fact]
        public void Times_Compare_LargerThen()
        {
            var time1 = new Time(1, 30);
            var time2 = new Time(1, 30);
            var timeMinuteLater = new Time(1, 31);
            var timeHourLater = new Time(2, 30);

            Assert.True(timeMinuteLater > time1);
            Assert.True(timeHourLater > time1);
            Assert.False(time1 > timeMinuteLater);
            Assert.False(time1 > time2);
        }

        [Fact]
        public void Times_Compare_SmallerThen()
        {
            var time1 = new Time(1, 30);
            var time2 = new Time(1, 30);
            var timeMinuteLater = new Time(1, 31);
            var timeHourLater = new Time(2, 30);

            Assert.False(timeMinuteLater < time1);
            Assert.False(time2 < time1);
            Assert.True(time1 < timeMinuteLater);
            Assert.True(time1 < timeHourLater);
        }

        [Fact]
        public void Times_Compare_LargerThenOrEqual()
        {
            var time1 = new Time(1, 30);
            var time2 = new Time(1, 30);
            var timeMinuteLater = new Time(1, 31);
            var timeHourLater = new Time(2, 30);

            Assert.True(timeMinuteLater >= time1);
            Assert.True(timeHourLater >= time1);
            Assert.False(time1 >= timeMinuteLater);
            Assert.True(time1 >= time2);
        }

        [Fact]
        public void Times_Compare_SmallerThenOrEqual()
        {
            var time1 = new Time(1, 30);
            var time2 = new Time(1, 30);
            var timeMinuteLater = new Time(1, 31);
            var timeHourLater = new Time(2, 30);

            Assert.False(timeMinuteLater <= time1);
            Assert.True(time2 <= time1);
            Assert.True(time1 <= timeMinuteLater);
            Assert.True(time1 <= timeHourLater);
        }
    }
}
