using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SibersProject.DAL.Entities;

namespace SibersProject.DAL.Data.Configurations;

public class ProjectDocumentConfiguration : IEntityTypeConfiguration<ProjectDocument>
{
    public void Configure(EntityTypeBuilder<ProjectDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(260);
        builder.Property(d => d.StoredFileName).IsRequired().HasMaxLength(260);
        builder.Property(d => d.ContentType).IsRequired().HasMaxLength(150);
        builder.Property(d => d.Size).IsRequired();
        builder.Property(d => d.UploadedAtUtc).IsRequired();

        builder.HasOne(d => d.Project)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ProjectDocuments");
    }
}
