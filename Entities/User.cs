using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IEEE.Entities
{
    public class User : IdentityUser<int>
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Faculty { get; set; }
        public string Year { get; set; } 
        public string Goverment { get;set; }
        public string Phone { get; set; }
        public string Sex { get; set; }
        public string Committee { get; set; }
        public string City { get; set; }
        public DateTime BirthOfDate { get;set; }  
        public bool IsActive { get; set; } = false;
        public string Role { get; set; }

        public int? CommitteeId { get; set; }

        public ICollection<Tasks>? HeadTasks { get; set; } = new List<Tasks>();
        public ICollection<Users_Tasks>? Users_Tasks { get; set; } = new List<Users_Tasks>();

        public ICollection<MeetingUser>? MeetingUsers { get; set; } = new List<MeetingUser>();


        // public ICollection<Meeting>? CreatorMeetings { get; set; } = new List<Meeting>();
        public ICollection<Meeting>? Meetings { get; set; } = new List<Meeting>();








    }
}
