namespace IEEE.Entities
{
    public class EventCategory
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }
        private readonly List<Event> _events = new();
        public IReadOnlyCollection<Event> Events => _events;

        private EventCategory() { } // For EF Core

        private EventCategory(string name, string? description)
        {
            Id = Guid.NewGuid();
            Name = name.Trim();
            Description = description?.Trim();
            CreatedAt = DateTime.UtcNow;
        }
        public static EventCategory Create(string name, string? description)
        {
            Validate(name);
            return new EventCategory(name, description);
        }
        
        public void Update(string name, string? description)
        {
            Validate(name);
            Name = name.Trim();
            Description = description?.Trim();
            LastUpdatedAt = DateTime.UtcNow;
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Event category name is required");
        }
    }
}