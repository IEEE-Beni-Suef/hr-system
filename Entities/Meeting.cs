namespace IEEE.Entities
{
    public class Meeting
    {
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Recap { get; set; }

        public int CommitteeId { get; set; }

        public Committee? Committee { get; set; }


        //public int CreatorId { get; set; }
        //public User? Creator { get; set; }

        public ICollection<MeetingUser>? MeetingUsers { get; set; } = new List<MeetingUser>();

        public ICollection<User>? Users { get; set; }
        
    }
}
