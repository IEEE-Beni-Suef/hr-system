namespace IEEE.DTO.CommitteeDto
{
    public class CommitteeGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HeadId { get; set; }
        public int? MemberCount { get; set; }
        public List<int> VicesId { get; set; }

        public string? ImageUrl { get; set; }

    }
}
