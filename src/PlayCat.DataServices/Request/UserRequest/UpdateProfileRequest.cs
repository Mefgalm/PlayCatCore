using System;
using System.ComponentModel.DataAnnotations;

namespace PlayCat.DataService.Request
{
    public class UpdateProfileRequest
    {
        public Guid Id { get; set; }

        [RegularExpression("^[a-zA-Z0-9_]{3,100}$", ErrorMessage = "FirstName allowed symbols A-Z, _ in range 3 to 100")]
        public string FirstName { get; set; }

        [RegularExpression("^[a-zA-Z0-9_]{3,100}$", ErrorMessage = "FirstName allowed symbols A-Z, _ in range 3 to 100")]
        public string LastName { get; set; }

        public string NickName { get; set; }
    }
}
