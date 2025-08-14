using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace IEEE.Configurations
{
    public class CommitteeConfig : IEntityTypeConfiguration<Committee> 
    {
        public void Configure(EntityTypeBuilder<Committee> builder)
        {


            // Head - علاقة واحد لواحد
            builder.HasOne(c => c.Head)
                .WithMany(u => u.HeadCommittees)
                .HasForeignKey(c => c.HeadId)
                .OnDelete(DeleteBehavior.SetNull);



            // جعل الـ Name فريد
            builder.HasIndex(c => c.Name)
                   .IsUnique();

            // جعل الـ HeadId فريد (رئيس واحد لا يمكن أن يرأس أكثر من لجنة)
            builder.HasIndex(c => c.HeadId)
                   .IsUnique();



        }


    }
}

