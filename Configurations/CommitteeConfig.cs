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


            //builder.HasOne(c => c.Head)
            //    .WithMany(h => h.HeadCommittees)
            //    .HasForeignKey(c => c.HeadId);


        }
    }

}
