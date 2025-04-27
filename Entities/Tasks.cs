namespace IEEE.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateOnly Month { get; set; }
        public int HeadId { get; set; }
        public User? Head { get; set; }

        // المفتاح الأجنبي لـ Committee
        public int CommitteeId { get; set; }

        // العلاقة مع Committee
        public Committee Committee { get; set; }

        public ICollection<Users_Tasks>? Users_Tasks { get; set; } = new List<Users_Tasks>();
    }
}
