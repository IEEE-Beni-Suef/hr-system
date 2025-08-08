using IEEE.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.UserDTO
{
    public class EditUserDto
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? NewPassword { get; set; }
        public Faculty Faculty {get; set; }
        public int RoleId { get; set; }
        public StudyYear Year { get; set; }
        public Goverment Goverment { get; set; }
        public string Phone { get; set; }
        public Sex Sex { get; set; }
        public List<int> CommitteeIds { get; set; } // IDs of selected committees
    }
}
