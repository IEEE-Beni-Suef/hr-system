using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace IEEE.Configurations
{
    public class EventConfig : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name)
                .HasColumnType("NVARCHAR")
                .IsRequired()
                .HasMaxLength(200);

            var keywordComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
                );

            builder.Property(e => e.KeyWords)
                .HasField("_keyWords")
                .HasConversion(
                k => JsonSerializer.Serialize(k, (JsonSerializerOptions?)null),
                k => JsonSerializer.Deserialize<List<string>>(k, (JsonSerializerOptions?)null) ?? new List<string>()
                );

            builder.HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Events");
        }
    }
}
