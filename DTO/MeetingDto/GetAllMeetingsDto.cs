using IEEE.DTO.UserDTO;

namespace IEEE.DTO.MeetingDto
{
    public class GetAllMeetingsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public string CommitteeName { get; set; }
        public string HeadUserName { get; set; }
        public List<GetUsersDto> Users { get; set; }  

    }
}
