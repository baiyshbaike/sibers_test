namespace SibersProject.BLL.DTOs.Project;

public class ProjectDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}
