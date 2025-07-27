using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.UserDTO
{
    public class LoginDto
    {


        [Required]
        public string UserName { get; set; }



        [Required]
        public string Password { get; set; }



        [Required]
        public int RoleId { get; set; }  
    }
}
