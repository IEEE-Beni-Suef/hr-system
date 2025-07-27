using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.UserDTO
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "UserName can only contain letters and digits.")]
        public string UserName { get; set; }

        [Required]
        public string FName { get; set; }


        [Required]
        public string MName { get; set; }


        [Required]
        public string LName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]

        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int CommitteeId { get; set; }

        public string Year { get; set; }
        public string Sex { get; set; }
        public string Faculty { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Goverment { get; set; }
        public string Role { get; set; }
        public string Committee { get; set; }



    }
}
