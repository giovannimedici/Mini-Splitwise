using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Infrastructure.Data.Configuration;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");  

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired();
        
        // builder.HasMany(g => g.Members) 
        //     .WithMany(m => m.Groups)
        //     .UsingEntity<GroupMember>(
        //         j => j.HasOne(gm => gm.Member)
        //             .WithMany()
        //             .HasForeignKey(gm => gm.MemberId),
        //         j => j.HasOne(gm => gm.Group)
        //             .WithMany()
        //             .HasForeignKey(gm => gm.GroupId)
        //     );
    }
}