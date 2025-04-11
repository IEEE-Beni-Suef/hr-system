using Microsoft.AspNetCore.Identity;

namespace IEEE.Entities
{
    public class User 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string City { get; set; }

        public int? CommitteeId { get; set; }

        
        public ICollection<Tasks>? HeadTasks { get; set; } = new List<Tasks>();
        public ICollection<Users_Tasks>? Users_Tasks { get; set; } = new List<Users_Tasks>();






    }
}
