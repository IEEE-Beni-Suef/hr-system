using Microsoft.AspNetCore.Identity;

namespace IEEE.Entities
{
    public class User : IdentityUser<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Faculty { get; set; }

        public string City { get; set; }

        public bool IsActive { get; set; } = false;

        public string Role { get; set; }



        public int? CommitteeId { get; set; }

        
        public ICollection<Tasks>? HeadTasks { get; set; } = new List<Tasks>();
        public ICollection<Users_Tasks>? Users_Tasks { get; set; } = new List<Users_Tasks>();
        public ICollection<meetings>? CreatorMeetings { get; set; } = new List<meetings>();
        public ICollection<meetings>? Meetings { get; set; } = new List<meetings>();






    }
}
