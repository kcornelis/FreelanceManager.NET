using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FreelanceManager
{
    public class ExtensionsTests
    {
        [Fact]
        public void Date_GetNumericValue()
        {
            Assert.Equal(20140102, new DateTime(2014, 01, 02).GetNumericValue());
            Assert.Equal(20140102, new DateTime(2014, 01, 02, 02, 03, 04).GetNumericValue());
        }

        [Fact]
        public void Dictionary_GetOrAdd_Gets_An_Existing_Value()
        {
            var dictionary = new Dictionary<int, string>();
            dictionary.Add(1, "1");

            Assert.Equal("1", dictionary.GetOrAdd(1, v => "test"));
        }

        [Fact]
        public void Dictionary_GetOrAdd_Adds_Unexisting_Key()
        {
            var dictionary = new Dictionary<int, string>();

            Assert.Equal("test", dictionary.GetOrAdd(1, v => "test"));
            Assert.Equal("test", dictionary.GetOrAdd(1, v => "test2"));
        }
    }
}
