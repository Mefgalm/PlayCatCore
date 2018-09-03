using System;
using System.ComponentModel.DataAnnotations;

namespace PlayCat.DataModels
{
    public class User
    {
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string NickName { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public DateTime RegisterDate { get; set; }

        public bool IsUploadingAudio { get; set; }  
        
        public virtual AuthToken AuthToken { get; set; }
    }
}
