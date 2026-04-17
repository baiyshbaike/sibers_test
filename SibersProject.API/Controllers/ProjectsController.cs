using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
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
            return project is null ? NotFound() : Ok(project);
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
                await _projectService.RemoveEmployeeAsync(projectId, employeeId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
