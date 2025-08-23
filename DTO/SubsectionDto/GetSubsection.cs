using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace IEEE.DTO.SubsectionDto
{
    public class GetSubsection
    {

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Subtitle { get; set; }

        [Required]
        public string Paragraph { get; set; }

        [StringLength(300)]
        public string Photo { get; set; }

        [Required]
        public int ArticleId { get; set; }
    }
}
