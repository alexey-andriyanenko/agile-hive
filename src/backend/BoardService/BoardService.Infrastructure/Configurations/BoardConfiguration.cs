using BoardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoardService.Infrastructure.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(x => new { x.Name, x.ProjectId })
            .IsUnique();
        
        builder.Property(x => x.CreatedByUserId)
            .IsRequired();
        
        builder.Property(x => x.ProjectId)
            .IsRequired();

        builder.HasOne(x => x.BoardType)
            .WithMany()
            .HasForeignKey(x => x.BoardTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        
        builder.HasMany(x => x.Columns)
            .WithOne(x => x.Board)
            .HasForeignKey(x => x.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}