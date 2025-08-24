using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.ArticleDto
{
    public class CreateArticleDto
    {

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Keywords { get; set; }

        public IFormFile ? Photo { get; set; } 


        [Required]
        public int CategoryId { get; set; }
    }
}
