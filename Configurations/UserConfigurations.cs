using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.FName).IsRequired();
            builder.Property(u => u.LName).IsRequired();
            builder.Property(u => u.Email).IsRequired();



            //builder.HasOne(u => u.Committee)
            //       .WithMany(c => c.Users)
            //       .HasForeignKey(u => u.CommitteeId);

        }
    }
}
