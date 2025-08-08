namespace IEEE.DTO.UserDTO
{
    public class GetUsersDto
    {

        public int Id { get; set; }
        public string Eamil { get; set; }
        public bool IsActive { get; set; }
        public int? CommitteeId { get; set; }
        public int ? RoleId { get; set; }



    }
}
