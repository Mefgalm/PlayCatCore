using System;

namespace PlayCat.ApiModels
{
    public class AuthToken
    {
        public Guid Id { get; set; }

        public DateTime DateExpired { get; set; }
    }
}
