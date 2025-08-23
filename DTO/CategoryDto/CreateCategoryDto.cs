using System.ComponentModel.DataAnnotations;

namespace IEEE.DTO.CategoryDto
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } 
    }
}
