using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Infrastructure.Data.Configuration;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired();
        
        builder.Property(m => m.Email)
            .IsRequired();

        builder.Property(m => m.BirthDate);
    }
}