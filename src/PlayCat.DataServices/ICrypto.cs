﻿using System.Collections.Generic;

namespace PlayCat.DataServices
{
    public interface ICrypto
    {
        byte[] GenerateSalt();
        byte[] HashPassword(byte[] salt, string password);
        bool IsValid(IEnumerable<byte> hashPassword, byte[] salt, string password);
    }
}