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


        public int? HeadId { get; set; }
        public User? Head { get; set; }

        public ICollection<Users_Meetings>? Users_Meetings { get; set; } = new List<Users_Meetings>();
        
    }
}
