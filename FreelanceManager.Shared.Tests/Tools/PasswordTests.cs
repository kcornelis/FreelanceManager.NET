using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FreelanceManager.Tools
{
    public class PasswordTests
    {
        [Fact]
        public void CreateRandomPassword_Genarates_Passwords()
        {
            Assert.NotEqual(Password.CreateRandomPassword(10), Password.CreateRandomPassword(10));
            Assert.NotEqual(Password.CreateRandomPassword(5), Password.CreateRandomPassword(5));
            Assert.NotEqual(Password.CreateRandomPassword(7), Password.CreateRandomPassword(7));
        }

        [Fact]
        public void CreateRandomPassword_Genarates_Passwords_With_A_Specified_Length()
        {
            Assert.Equal(10, Password.CreateRandomPassword(10).Length);
            Assert.Equal(5, Password.CreateRandomPassword(5).Length);
            Assert.Equal(2, Password.CreateRandomPassword(2).Length);
        }

        [Fact]
        public void CreateRandomSalt_Genarates_Random_Numbers()
        {
            Assert.NotEqual(Password.CreateRandomSalt(), Password.CreateRandomSalt());
            Assert.NotEqual(Password.CreateRandomSalt(), Password.CreateRandomSalt());
        }

        [Fact]
        public void ComputeSaltedHash_Hashes_The_Password()
        {
            var password = "test123";
            var hasher = new Password(password, 30);

            Assert.NotEqual("test123", hasher.ComputeSaltedHash());
        }

        [Fact]
        public void EqualsSaltedHash_Compares_A_HashedPassword()
        {
            var password = "test123";

            var hashed1 = new Password(password, 30).ComputeSaltedHash();
            var hashed2 = new Password(password, 33).ComputeSaltedHash();
            var hashed3 = new Password("aaa", 30).ComputeSaltedHash();

            Assert.True(new Password(password, 30).EqualsSaltedHash(hashed1));
            Assert.False(new Password(password, 30).EqualsSaltedHash(hashed2));
            Assert.False(new Password(password, 30).EqualsSaltedHash(hashed3));
        }
    }
}
