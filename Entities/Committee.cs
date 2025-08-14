namespace IEEE.Entities
{
    public class Committee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? memberCount { get; set; } 
        public string ? ImageUrl { get; set; } 
       public string ? Description { get; set; }
        public int? HeadId { get; set; }
        public User? Head { get; set; }



        

        public ICollection<User> Vices { get; set; } = new List<User>(); // أكتر من نائب
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();



    }
}
