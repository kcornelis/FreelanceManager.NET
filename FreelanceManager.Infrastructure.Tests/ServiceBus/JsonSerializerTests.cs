﻿using System;
using System.Collections.Generic;
using System.Linq;
using FreelanceManager.Events.TimeRegistration;
using Xunit;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    public class JsonSerializerTests
    {
        [Fact]
        public void Basic_Object_Conversion()
        {
            var toTest = new Person("John", "Doe", 40);
            var result = Test(toTest);

            Assert.False(object.ReferenceEquals(toTest, result));

            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal(40, result.Age);
        }

        [Fact]
        public void Object_With_Array_Conversion()
        {
            var toTest = new ArrayTest
            {
                Array = new[]{ 
                     new Person("John", "Doe", 40),
                     new Person("Jane", "Doe", 30)
                }
            };

            var result = Test(toTest);

            Assert.False(object.ReferenceEquals(toTest, result));

            Assert.Equal("John", ((Person)result.Array.First()).FirstName);
            Assert.Equal("Doe", ((Person)result.Array.First()).LastName);
            Assert.Equal(40, ((Person)result.Array.First()).Age);

            Assert.Equal("Jane", ((Person)result.Array.Last()).FirstName);
            Assert.Equal("Doe", ((Person)result.Array.Last()).LastName);
            Assert.Equal(30, ((Person)result.Array.Last()).Age);
        }

        [Fact]
        public void FreelanceManager_Objects_Conversion()
        {
            var toTest = new FMTest
            {
                Date = new Date(2013, 10, 12),
                Time = new Time(10, 12),
                Money = 30M
            };

            var result = Test(toTest);

            Assert.False(object.ReferenceEquals(toTest, result));

            Assert.Equal(new Date(2013, 10, 12), result.Date);
            Assert.Equal(new Time(10, 12), result.Time);
            Assert.Equal(new Money(30M), result.Money);
        }

        [Fact]
        public void Rate_Bug_Test__Cant_Parse_Decimal()
        {
            // BUG fixed, the constructor should contain a money type and not a decimal type
            // (Guid id, decimal rate) => NOK     (Guid id, Money rate) => OK
            var toTest = new DomainUpdateBusMessage();
            toTest.Events = new[]
            { 
                JsonSerializer.Serialize(new TimeRegistrationRateRefreshed(Guid.NewGuid(), 10)) 
            };

            var result = Test(toTest);

            Assert.False(object.ReferenceEquals(toTest, result));

            var rateRefreshedEvent = (TimeRegistrationRateRefreshed)JsonSerializer.Deserialize(result.Events.First());
            Assert.Equal(new Money(10), rateRefreshedEvent.Rate);
        }

        private T Test<T>(T objectToTest)
        {
            var json = JsonSerializer.Serialize(objectToTest);
            return (T)JsonSerializer.Deserialize(json);
        }

        class Person
        {
            public Person(string fn, string ln, int a)
            {
                FirstName = fn;
                LastName = ln;
                Age = a;
            }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }

        class ArrayTest
        {
            public object[] Array { get; set; }
        }

        class FMTest
        {
            public Date Date { get; set; }
            public Time Time { get; set; }
            public Money Money { get; set; }
        }
    }
}
