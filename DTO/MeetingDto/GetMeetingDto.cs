using IEEE.DTO.CommitteeDto;
using IEEE.DTO.UserDTO;

namespace IEEE.DTO.MeetingDto
{
    public class GetMeetingDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Recap { get; set; }
        public CommitteeGetDto Committee { get; set; }
        public GetUsersDto Head { get; set; }
        public List<GetUsersDto> Users { get; set; }


    }
}
