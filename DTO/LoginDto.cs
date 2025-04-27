using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO
{
    public class LoginDto
    {


        [Required]
        public string UserName { get; set; }



        [Required]
        public string Password { get; set; }



        [Required]
        public string Role { get; set; }  
    }
}
