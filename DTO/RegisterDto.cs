using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO
{
    public class RegisterDto
    {

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }


        [Required]
        public string Name { get; set; }


        [Required]
        [StringLength(100, MinimumLength = 6)]

        public string Password { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        public string Faculty { get; set; }


        [Required]
        public string Role { get; set; } 
    }
}
