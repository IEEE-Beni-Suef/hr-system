namespace IEEE.DTO.EventsDTO;

public record UpdateEventRequest (
    string? Name,
    IEnumerable<string>? KeyWords,
    DateTime? StartDate,
    DateTime? EndDate,
    bool IsCommingSoon,
    Guid? CategoryId
);