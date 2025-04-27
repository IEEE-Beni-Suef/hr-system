using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO
{
    public class EditUserDto
    {
        public string UserName { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Faculty { get; set; }
        public string Role { get; set; }
        public string Year { get; set; }
        public string Goverment { get; set; }
        public string Phone { get; set; }
        public string Sex { get; set; }
        public string Committee { get; set; }
      //  public DateTime BirthOfDate { get; set; }
    }
}
