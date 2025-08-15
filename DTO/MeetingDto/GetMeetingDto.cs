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
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public string CommitteeName { get; set; }
        public string HeadName { get; set; }


    }
}
