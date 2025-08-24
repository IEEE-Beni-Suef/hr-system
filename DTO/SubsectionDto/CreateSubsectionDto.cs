using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.SubsectionDto
{
    public class CreateSubsectionDto
    {
        [Required]
        [StringLength(200)]
        public string Subtitle { get; set; }

        [Required]
        public string Paragraph { get; set; }

        public IFormFile? Photo { get; set; }

        [Required]
        public int ArticleId { get; set; }
    }
}
