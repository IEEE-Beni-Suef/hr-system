using System.ComponentModel.DataAnnotations;

namespace IEEE.Entities
{
    public class Sponsor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? Date { get; set; }

        public string? Image { get; set; }
    }
}
