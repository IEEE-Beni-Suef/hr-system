namespace IEEE.DTO.MeetingDto
{
    public class CreateMeetingDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Recap { get; set; }
        public string Type { get; set; } // online or offline
        public DateTime DateTime { get; set; }
        public int CommitteeId { get; set; }
        public int HeadId { get; set; }
    }
}
