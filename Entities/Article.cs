using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IEEE.Entities
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Keywords { get; set; }

        public string ? Photo { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public  Category Category { get; set; }

        public  ICollection<Subsection> Subsections { get; set; } = new List<Subsection>();



    }
}
