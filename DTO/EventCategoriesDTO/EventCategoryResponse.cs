namespace IEEE.DTO.EventCategoriesDTO;

// Response DTO
public record EventCategoryResponse(Guid Id, string Name, string? Description, DateTime CreatedAt, DateTime? LastUpdatedAt);