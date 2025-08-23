namespace IEEE.DTO.ArticleDto
{
    public class GetArticle
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Photo { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
