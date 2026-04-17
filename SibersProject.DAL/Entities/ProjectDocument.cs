namespace SibersProject.DAL.Entities;

// #документ_проекта / #project_document
// Physical file metadata for project attachments.
public class ProjectDocument
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
