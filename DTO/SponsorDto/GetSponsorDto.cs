namespace IEEE.DTO.SponsorDto
{
    public class GetSponsorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string? Image { get; set; }
    }
}
