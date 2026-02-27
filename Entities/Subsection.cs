using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IEEE.Entities
{
    public class Subsection
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Subtitle { get; set; }

        [Required]
        public string Paragraph { get; set; }


        public string? Photo { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [ForeignKey("ArticleId")]
        public Article Article { get; set; }









    }
}