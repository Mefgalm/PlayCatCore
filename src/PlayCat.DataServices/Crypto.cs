using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PlayCat.DataServices
{
    public class Crypto : ICrypto
    {
        public byte[] GenerateSalt()
        {
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public byte[] HashPassword(byte[] salt, string password)
        {
            return KeyDerivation.Pbkdf2(password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
        }

        public bool IsValid(IEnumerable<byte> hashPassword, byte[] salt, string password)
        {
            return HashPassword(salt, password).SequenceEqual(hashPassword);
        }
    }
}