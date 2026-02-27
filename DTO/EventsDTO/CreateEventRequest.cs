namespace IEEE.DTO.EventsDTO
{
    public record CreateEventRequest(string Name, IEnumerable<string> KeyWords, DateTime? StartDate, DateTime? EndDate, bool IsCommingSoon, Guid CategoryId);

}
