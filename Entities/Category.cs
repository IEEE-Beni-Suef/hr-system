using System.ComponentModel.DataAnnotations;

namespace IEEE.Entities
{
    public class Category
    {

        [Key]
        public int    Id { get; set; }
        public string Name { get; set; }


        public  ICollection<Article> Articles { get; set; } = new List<Article>();













    }
}
