namespace IEEE.Entities
{
    public class Users_Meetings
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public int MeetingId { get; set; }
        public Meeting? Meeting { get; set; }
    }
}
