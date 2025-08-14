namespace IEEE.DTO.UserDTO
{
    public class GetUsersDto
    {

        public int Id { get; set; }
        public string Eamil { get; set; }
        public bool IsActive { get; set; }
        public List<int> CommitteesId { get; set; }
        public int ? RoleId { get; set; }



    }
}
