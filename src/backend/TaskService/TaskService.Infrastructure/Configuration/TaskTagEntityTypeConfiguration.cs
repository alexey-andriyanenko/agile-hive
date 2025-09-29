using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Configuration;

public class TaskTagEntityTypeConfiguration : IEntityTypeConfiguration<TaskTagEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskTagEntity> builder)
    {
        builder.HasKey(x => new { x.TaskId, x.TagId });

        builder.HasOne(x => x.Task)
            .WithMany(x => x.TaskTags)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.TaskTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("TaskTags");
    }
}