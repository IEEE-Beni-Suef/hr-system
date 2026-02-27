namespace IEEE.DTO.EventsDTO
{
    public record EventResponse(Guid Id, string Name, IEnumerable<string> KeyWords, DateTime? StartDate, DateTime? EndDate, bool IsCommingSoon, Guid CategoryId, string CategoryName, DateTime CreatedAt, DateTime? LastUpdatedAt);

}
