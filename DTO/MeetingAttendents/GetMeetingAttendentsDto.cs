namespace IEEE.DTO.MeetingAttendents
{
    public class GetMeetingAttendentsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAttend { get; set; }
        public int Score { get; set; }
    }
}
