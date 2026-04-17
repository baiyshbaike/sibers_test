namespace SibersProject.BLL.DTOs.Project;

public class CreateProjectDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}
