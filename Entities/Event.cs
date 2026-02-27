namespace IEEE.Entities
{
    public class Event
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;

        private  List<string> _keyWords = new();
        public IReadOnlyCollection<string> KeyWords => _keyWords;
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsCommingSoon { get; private set; }
        public Guid CategoryId { get; private set; }
        public EventCategory Category { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        private Event() { } // For EF Core

        private Event(string name, IEnumerable<string> keyWords, DateTime? startDate, DateTime? endDate, bool isCommingSoon, Guid categoryId)
        {
            Id = Guid.NewGuid();
            Name = name.Trim();
            _keyWords = keyWords?.Select(k => k.Trim()).Where(k => !string.IsNullOrWhiteSpace(k)).ToList() ?? new();
            StartDate = startDate;
            EndDate = endDate;
            IsCommingSoon = isCommingSoon;
            CategoryId = categoryId;
            CreatedAt = DateTime.UtcNow;
        }
        public static Event Create(string name, IEnumerable<string> keyWords, DateTime? startDate, DateTime? endDate, bool isCommingSoon, Guid categoryId)
        {
            Validate(name, startDate, endDate, isCommingSoon);
            return new Event(name, keyWords, startDate, endDate, isCommingSoon, categoryId);
        }
        public void Update(string name, IEnumerable<string> keyWords, DateTime? startDate, DateTime? endDate, bool isCommingSoon, Guid categoryId)
        {
            Validate(name, startDate, endDate, isCommingSoon);

            Name = name.Trim();
            _keyWords = keyWords?.Select(k => k.Trim()).Where(k => !string.IsNullOrWhiteSpace(k)).ToList() ?? new();
            StartDate = startDate;
            EndDate = endDate;
            IsCommingSoon = isCommingSoon;
            CategoryId = categoryId;
            LastUpdatedAt = DateTime.UtcNow;
        }
        private static void Validate(string name, DateTime? startDate, DateTime? endDate, bool isCommingSoon)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Event name is required");
            if (isCommingSoon)
            {
                if (startDate.HasValue || endDate.HasValue)
                    throw new ArgumentException("StartDate and EndDate must be null when IsCommingSoon is true");
                else
                {
                    if (!startDate.HasValue)
                        throw new ArgumentException("StartDate is required when not comming soon");
                    if (!endDate.HasValue && endDate < startDate)
                        throw new ArgumentException("EndDate must be greater than StartDate");
                }
            }
        }
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Event name is required");
            Name = newName.Trim();
            LastUpdatedAt = DateTime.UtcNow;
        }
        public void AddKeyWord(string keyWord)
        {
            if (string.IsNullOrWhiteSpace(keyWord))
                throw new ArgumentException("Keyword cannot be empty");
            _keyWords.Add(keyWord.Trim());
            LastUpdatedAt = DateTime.UtcNow;
        }
        public void UpdateKeyWords(IEnumerable<string> keyWords)
        {
            _keyWords.Clear();
            _keyWords = keyWords?.Select(k => k.Trim()).Where(k => !string.IsNullOrWhiteSpace(k)).ToList() ?? new();
            LastUpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDates(DateTime? startDate, DateTime? endDate, bool isCommingSoon)
        {
            if (isCommingSoon)
            {
                if (startDate.HasValue || endDate.HasValue)
                    throw new ArgumentException("StartDate and EndDate must be null when IsCommingSoon is true");
            }
            else
            {
                if (!startDate.HasValue)
                    throw new ArgumentException("StartDate is required when not comming soon");
                if (!endDate.HasValue && endDate < startDate)
                    throw new ArgumentException("EndDate must be greater than StartDate");
            }
            StartDate = startDate;
            EndDate = endDate;
            IsCommingSoon = isCommingSoon;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
