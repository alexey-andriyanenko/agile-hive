using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Configuration;

public class TaskEntityTypeConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(x => new { x.TenantId, x.ProjectId });
        builder.HasIndex(x => new { x.BoardId, x.BoardColumnId });
        builder.HasIndex(x => x.Title);
        
        builder.Property(x => x.Description)
            .HasColumnType("jsonb");
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .ValueGeneratedOnUpdate();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TaskTags)
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Tasks");
    }
}