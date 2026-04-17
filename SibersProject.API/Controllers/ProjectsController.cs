using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SibersProject.BLL.DTOs.Project;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Entities.Identity;
using SibersProject.DAL.Filters;

namespace SibersProject.API.Controllers
{
    // #контроллер_проектов / #projects_controller
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IWebHostEnvironment _env;

        public ProjectsController(
            IProjectService projectService,
            IWebHostEnvironment env)
        {
            _projectService = projectService;
            _env = env;
        }

        /// <summary>
        /// Get filtered and sorted projects
        /// Получить отфильтрованный и отсортированный список проектов
        /// </summary>
        // GET projects: Supervisor, ProjectManager (own via filter), Employee (own via filter)
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager + "," + ApplicationRoles.Employee)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ProjectFilter filter)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var employeeId = TryGetEmployeeIdFromClaims();

            if (role == ApplicationRoles.ProjectManager)
            {
                if (!employeeId.HasValue) return Forbid();
                filter.ProjectManagerId = employeeId.Value;
            }
            else if (role == ApplicationRoles.Employee)
            {
                if (!employeeId.HasValue) return Forbid();
                filter.AssignedEmployeeId = employeeId.Value;
            }

            var projects = await _projectService.GetAllAsync(filter);
            return Ok(projects);
        }

        /// <summary>Get project with details / Получить проект с подробностями</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager + "," + ApplicationRoles.Employee)]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project is null) return NotFound();
            if (!CanAccessProject(project)) return Forbid();
            return Ok(project);
        }

        /// <summary>Create new project / Создать новый проект</summary>
        // POST/PUT/DELETE project: Supervisor, ProjectManager
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpPost]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!CanManageProjectByEmployee(dto.ProjectManagerId))
                    return Forbid();

                var created = await _projectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Update project / Обновить проект</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existing = await _projectService.GetByIdAsync(id);
                if (existing is null) return NotFound();
                if (!CanManageProject(existing)) return Forbid();
                if (!CanManageProjectByEmployee(dto.ProjectManagerId)) return Forbid();

                var updated = await _projectService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Delete project / Удалить проект</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var existing = await _projectService.GetByIdAsync(id);
                if (existing is null) return NotFound();
                if (!CanManageProject(existing)) return Forbid();

                await _projectService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add employee to project / Добавить сотрудника в проект
        /// </summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpPost("{projectId:guid}/employees/{employeeId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddEmployee(Guid projectId, Guid employeeId)
        {
            try
            {
                var existing = await _projectService.GetByIdAsync(projectId);
                if (existing is null) return NotFound();
                if (!CanManageProject(existing)) return Forbid();

                await _projectService.AddEmployeeAsync(projectId, employeeId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove employee from project / Удалить сотрудника из проекта
        /// </summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpDelete("{projectId:guid}/employees/{employeeId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveEmployee(Guid projectId, Guid employeeId)
        {
            try
            {
                var existing = await _projectService.GetByIdAsync(projectId);
                if (existing is null) return NotFound();
                if (!CanManageProject(existing)) return Forbid();

                await _projectService.RemoveEmployeeAsync(projectId, employeeId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Upload project documents / Загрузить документы проекта</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpPost("{id:guid}/documents")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadDocuments(Guid id, [FromForm] List<IFormFile> files)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project is null) return NotFound();
            if (!CanManageProject(project)) return Forbid();
            if (files.Count == 0) return BadRequest(new { message = "No files uploaded." });

            var basePath = Path.Combine(_env.ContentRootPath, "Storage", "ProjectDocuments", id.ToString());
            Directory.CreateDirectory(basePath);
            var records = new List<CreateProjectDocumentDto>();

            foreach (var file in files.Where(f => f.Length > 0))
            {
                var docId = Guid.NewGuid();
                var ext = Path.GetExtension(file.FileName);
                var stored = $"{docId}{ext}";
                var fullPath = Path.Combine(basePath, stored);

                await using var stream = System.IO.File.Create(fullPath);
                await file.CopyToAsync(stream);

                records.Add(new CreateProjectDocumentDto
                {
                    Id = docId,
                    FileName = file.FileName,
                    StoredFileName = stored,
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    Size = file.Length,
                    UploadedAtUtc = DateTime.UtcNow
                });
            }

            if (records.Count > 0)
                await _projectService.AddDocumentsAsync(id, records);
            return NoContent();
        }

        /// <summary>Download project document / Скачать документ проекта</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager + "," + ApplicationRoles.Employee)]
        [HttpGet("{id:guid}/documents/{documentId:guid}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadDocument(Guid id, Guid documentId)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project is null) return NotFound();
            if (!CanAccessProject(project)) return Forbid();

            var doc = await _projectService.GetDocumentForDownloadAsync(id, documentId);
            if (doc is null) return NotFound();

            var fullPath = Path.Combine(_env.ContentRootPath, "Storage", "ProjectDocuments", id.ToString(), doc.StoredFileName);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, doc.ContentType, doc.FileName);
        }

        /// <summary>Delete project document / Удалить документ проекта</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpDelete("{id:guid}/documents/{documentId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(Guid id, Guid documentId)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project is null) return NotFound();
            if (!CanManageProject(project)) return Forbid();

            var doc = await _projectService.DeleteDocumentAsync(id, documentId);
            if (doc is null) return NotFound();

            var fullPath = Path.Combine(_env.ContentRootPath, "Storage", "ProjectDocuments", id.ToString(), doc.StoredFileName);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            return NoContent();
        }

        private Guid? TryGetEmployeeIdFromClaims()
        {
            var value = User.FindFirstValue("employee_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }

        private bool IsSupervisor() => User.IsInRole(ApplicationRoles.Supervisor);

        private bool CanAccessProject(ProjectDto project)
        {
            if (IsSupervisor()) return true;

            var employeeId = TryGetEmployeeIdFromClaims();
            if (!employeeId.HasValue) return false;

            if (User.IsInRole(ApplicationRoles.ProjectManager))
                return project.ProjectManager?.Id == employeeId.Value;

            if (User.IsInRole(ApplicationRoles.Employee))
                return project.ProjectManager?.Id == employeeId.Value
                    || project.Employees.Any(e => e.Id == employeeId.Value);

            return false;
        }

        private bool CanManageProject(ProjectDto project)
        {
            if (IsSupervisor()) return true;

            if (User.IsInRole(ApplicationRoles.ProjectManager))
            {
                var employeeId = TryGetEmployeeIdFromClaims();
                return employeeId.HasValue && project.ProjectManager?.Id == employeeId.Value;
            }

            return false;
        }

        private bool CanManageProjectByEmployee(Guid projectManagerId)
        {
            if (IsSupervisor()) return true;

            if (User.IsInRole(ApplicationRoles.ProjectManager))
            {
                var employeeId = TryGetEmployeeIdFromClaims();
                return employeeId.HasValue && projectManagerId == employeeId.Value;
            }

            return false;
        }
    }
}
