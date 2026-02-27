using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class EventCategoryConfig : IEntityTypeConfiguration<EventCategory>
    {
        public void Configure(EntityTypeBuilder<EventCategory> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500); 

            builder.Metadata.FindNavigation(nameof(EventCategory.Events))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.ToTable("EventCategories");
        }
    }
}
