namespace IEEE.Entities
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateOnly Month { get; set; }
        public int HeadId { get; set; }                   
        public User? Head { get; set; }

        
        public int CommitteeId { get; set; }
        public Committee? Committee { get; set; }

        public ICollection<Users_Tasks>? Users_Tasks { get; set; } = new List<Users_Tasks>();
    }
}
