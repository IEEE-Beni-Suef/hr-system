using IEEE.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.UserDTO
{
    public class RegisterDto
    {

       

        [Required]
        public string FirstName { get; set; }


        [Required]
        public string MiddleName { get; set; }


        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]

        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public List<int> CommitteeIds { get; set; }      // IDs للكوميتيز اللي هيختارها

        public StudyYear Year { get; set; }
        public Sex Sex { get; set; }
        public Faculty Faculty { get; set; }
        public string Phone { get; set; }
        public Goverment Goverment { get; set; }
        public int RoleId { get; set; }




    }
}
