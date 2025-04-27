using Microsoft.AspNetCore.Identity;

namespace IEEE.Entities
{
    public class Role : IdentityRole<int>
    {
        public int  Id { get; set; }
        public string Description { get; set; }
    }
}
