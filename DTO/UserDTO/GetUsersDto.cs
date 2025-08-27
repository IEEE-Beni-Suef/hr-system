using IEEE.Entities.Enums;

namespace IEEE.DTO.UserDTO
{
    public class GetUsersDto
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<int> CommitteesId { get; set; }

        public int ? RoleId { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Sex Sex { get; set; }
        public string PhoneNumber { get; set; }
        public Goverment Goverment { get; set; }
        public Faculty Faculty { get; set; }
        public StudyYear Year { get; set; }


    }
}
