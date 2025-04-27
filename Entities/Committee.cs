namespace IEEE.Entities
{
    public class Committee
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public ICollection<Meeting>? Meetings { get; set; } = new List<Meeting>();
        public ICollection<Tasks>? Tasks { get; set; } = new List<Tasks>();

    }
}
