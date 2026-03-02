using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.SponsorDto
{
    public class CreateSponsorDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? Date { get; set; }

        public IFormFile? Image { get; set; }
    }
}
