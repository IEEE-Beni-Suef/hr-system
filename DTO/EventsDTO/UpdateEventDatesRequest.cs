namespace IEEE.DTO.EventsDTO
{
    public record UpdateEventDatesRequest(DateTime? StartDate, DateTime? EndDate, bool IsCommingSoon);

}
