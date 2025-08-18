namespace IEEE.DTO.MeetingAttendents
{
    public class CreateAttendentsDto
    {
        // UserAttendent is the class that hold Id and isAttend and Score for each user
        public List<UserAttendent> UsersAttendents { get; set; }
        public int MeetingId { get; set; }

    }
}
