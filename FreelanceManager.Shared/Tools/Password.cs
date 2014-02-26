using System;
using System.Security.Cryptography;
using System.Text;

namespace FreelanceManager.Tools
{
    public class Password
    {
        private readonly string _password;
        private readonly int _salt;

        public Password(string strPassword, int nSalt)
        {
            _password = strPassword;
            _salt = nSalt;
        }

        public bool EqualsSaltedHash(string passwordHash)
        {
            var computedPassword = ComputeSaltedHash();
            return computedPassword == passwordHash;
        }

        public string ComputeSaltedHash()
        {
            // Create Byte array of password string
            var encoder = new ASCIIEncoding();
            Byte[] _secretBytes = encoder.GetBytes(_password);

            // Create a new salt
            var _saltBytes = new Byte[4];
            _saltBytes[0] = (byte)(_salt >> 24);
            _saltBytes[1] = (byte)(_salt >> 16);
            _saltBytes[2] = (byte)(_salt >> 8);
            _saltBytes[3] = (byte)(_salt);

            // append the two arrays
            var toHash = new Byte[_secretBytes.Length + _saltBytes.Length];
            Array.Copy(_secretBytes, 0, toHash, 0, _secretBytes.Length);
            Array.Copy(_saltBytes, 0, toHash, _secretBytes.Length, _saltBytes.Length);

            SHA1 sha1 = SHA1.Create();
            Byte[] computedHash = sha1.ComputeHash(toHash);

            return encoder.GetString(computedHash);
        }

        public static int CreateRandomSalt()
        {
            var _saltBytes = new Byte[4];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(_saltBytes);

            return (((_saltBytes[0]) << 24) + ((_saltBytes[1]) << 16) +
                    ((_saltBytes[2]) << 8) + (_saltBytes[3]));
        }

        public static string CreateRandomPassword(int passwordLength)
        {
            var _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
            var randomBytes = new Byte[passwordLength];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            var chars = new char[passwordLength];
            var allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = _allowedChars[randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
    }
}