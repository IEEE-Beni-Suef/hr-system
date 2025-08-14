using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class MeetingConfig : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(m => m.Title).IsRequired();
            builder.Property(m => m.Description).IsRequired();
            builder.Property(m => m.Recap).IsRequired();

            builder.HasOne(m => m.Committee)
                .WithMany(c => c.Meetings)
                .HasForeignKey(m => m.CommitteeId);


            builder.HasOne(m => m.Head)
                .WithMany(h => h.HeadMeetings)
                .HasForeignKey(m => m.HeadId);


            //builder.HasMany(u => u.Users)
            // .WithMany(m => m.Meetings)
            // .UsingEntity<Dictionary<string, object>>(
            //     "Users_Meetings",
            //     j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
            //     j => j.HasOne<Meeting>().WithMany().HasForeignKey("MeetingId").OnDelete(DeleteBehavior.Restrict)

            //     );


        }
    }
}
