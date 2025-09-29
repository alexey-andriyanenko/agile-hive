using BoardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardService.Infrastructure.Configurations;

public class BoardTypeConfiguration : IEntityTypeConfiguration<BoardType>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BoardType> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(x => x.Name)
            .IsUnique();
        
        builder.Property(x => x.IsEssential)
            .IsRequired();
        
        builder.HasMany(x => x.Boards)
            .WithOne(x => x.BoardType)
            .HasForeignKey(x => x.BoardTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}