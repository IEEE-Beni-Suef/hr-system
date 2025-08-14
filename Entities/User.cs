using IEEE.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IEEE.Entities
{
    public class User : IdentityUser<int>
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public Faculty Faculty { get; set; }
        public StudyYear Year { get; set; } 
        public Goverment Goverment { get;set; }
        public Sex Sex { get; set; }
        public bool IsActive { get; set; } = false;
        public int? CommitteeId { get; set; }
        public int RoleId { get; set; }
        public virtual ApplicationRole Role { get; set; }



        // اللجنة اللي هو نائب فيها
        public int? ViceCommitteeId { get; set; }
        public Committee? ViceCommittee { get; set; }


        public ICollection<Tasks>? HeadTasks { get; set; } = new List<Tasks>();
        public ICollection<Users_Tasks>? Users_Tasks { get; set; } = new List<Users_Tasks>();

       //  public ICollection<MeetingUser>? MeetingUsers { get; set; } = new List<MeetingUser>();
         //public ICollection<Meeting>? CreatorMeetings { get; set; } = new List<Meeting>();
        public ICollection<Meeting>? Meetings { get; set; } = new List<Meeting>();
        public ICollection<Committee> Committees { get; set; } = new List<Committee>();

        public ICollection<Committee> HeadCommittees { get; set; } = new List<Committee>();

        public ICollection<Meeting> HeadMeetings { get; set; } = new List<Meeting>();

    }
}
