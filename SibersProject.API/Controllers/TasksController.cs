using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SibersProject.BLL.DTOs.Task;
using SibersProject.BLL.DTOs.Project;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Entities.Identity;
using SibersProject.DAL.Filters;

namespace SibersProject.API.Controllers
{
    // #контроллер_задач / #tasks_controller
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(
            ITaskService taskService,
            IProjectService projectService,
            UserManager<ApplicationUser> userManager)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userManager = userManager;
        }

        /// <summary>Get filtered tasks / Получить отфильтрованные задачи</summary>
        // GET tasks: Supervisor, ProjectManager, Employee (own via filter)
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager + "," + ApplicationRoles.Employee)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] TaskFilter filter)
        {
            var employeeId = TryGetEmployeeIdFromClaims();
            if (User.IsInRole(ApplicationRoles.ProjectManager))
            {
                if (!employeeId.HasValue) return Forbid();
                filter.ProjectManagerId = employeeId.Value;
            }
            else if (User.IsInRole(ApplicationRoles.Employee))
            {
                if (!employeeId.HasValue) return Forbid();
                filter.ExecutorId = employeeId.Value;
            }

            var tasks = await _taskService.GetAllAsync(filter);
            return Ok(tasks);
        }

        /// <summary>Get task by ID / Получить задачу по ID</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager + "," + ApplicationRoles.Employee)]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task is null) return NotFound();
            if (!await CanAccessTaskAsync(task)) return Forbid();
            return Ok(task);
        }

        /// <summary>Create new task / Создать новую задачу</summary>
        // POST/DELETE task: Supervisor, ProjectManager
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpPost]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateTaskItemDto dto)
        {
            if (dto is null) return BadRequest(new { message = "Request body is required." });
            try
            {
                if (!await CanManageProjectByIdAsync(dto.ProjectId))
                    return Forbid();

                var currentEmployeeId = TryGetEmployeeIdFromClaims();
                if (User.IsInRole(ApplicationRoles.ProjectManager))
                {
                    if (!currentEmployeeId.HasValue)
                        return BadRequest(new { message = "Current manager is not linked to employee profile." });
                    // Project manager can author only as themselves.
                    dto.AuthorId = currentEmployeeId.Value;
                }
                else if (User.IsInRole(ApplicationRoles.Supervisor))
                {
                    // Supervisor can pick any manager as author.
                    if (!await IsProjectManagerAsync(dto.AuthorId))
                        return BadRequest(new { message = "Author must be a user with ProjectManager role." });
                }
                else
                {
                    return Forbid();
                }

                if (dto.ExecutorId.HasValue && !await IsEmployeeAsync(dto.ExecutorId.Value))
                    return BadRequest(new { message = "Executor must be a user with Employee role." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _taskService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Update task / Обновить задачу</summary>
        // PUT task (incl. status): Supervisor, ProjectManager, Employee
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager + "," + ApplicationRoles.Employee)]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existing = await _taskService.GetByIdAsync(id);
                if (existing is null) return NotFound();
                if (!await CanAccessTaskAsync(existing)) return Forbid();

                if (User.IsInRole(ApplicationRoles.Employee))
                {
                    // Employees can only change status of their own tasks.
                    // Сотрудник может менять только статус своих задач.
                    dto = new UpdateTaskItemDto
                    {
                        Name = existing.Name,
                        Comment = existing.Comment,
                        Priority = existing.Priority,
                        ExecutorId = existing.Executor?.Id,
                        Status = dto.Status
                    };
                }
                else if (User.IsInRole(ApplicationRoles.ProjectManager))
                {
                    if (!await CanManageProjectByIdAsync(existing.ProjectId))
                        return Forbid();
                    // Additional check: Project Manager can only edit tasks in their own projects
                    // Дополнительная проверка: Project Manager может редактировать только задачи в своих проектах
                    var currentEmployeeId = TryGetEmployeeIdFromClaims();
                    if (!currentEmployeeId.HasValue)
                        return Forbid();
                    var project = await _projectService.GetByIdAsync(existing.ProjectId);
                    if (project?.ProjectManager?.Id != currentEmployeeId.Value)
                        return Forbid();
                }

                if (dto.ExecutorId.HasValue && !await IsEmployeeAsync(dto.ExecutorId.Value))
                    return BadRequest(new { message = "Executor must be a user with Employee role." });

                var updated = await _taskService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Delete task / Удалить задачу</summary>
        [Authorize(Roles = ApplicationRoles.Supervisor + "," + ApplicationRoles.ProjectManager)]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var existing = await _taskService.GetByIdAsync(id);
                if (existing is null) return NotFound();
                if (!await CanManageProjectByIdAsync(existing.ProjectId))
                    return Forbid();

                await _taskService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private Guid? TryGetEmployeeIdFromClaims()
        {
            var value = User.FindFirstValue("employee_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }

        private async Task<bool> CanManageProjectByIdAsync(Guid projectId)
        {
            if (User.IsInRole(ApplicationRoles.Supervisor)) return true;
            if (!User.IsInRole(ApplicationRoles.ProjectManager)) return false;

            var employeeId = TryGetEmployeeIdFromClaims();
            if (!employeeId.HasValue) return false;

            var project = await _projectService.GetByIdAsync(projectId);
            return project is not null && project.ProjectManager?.Id == employeeId.Value;
        }

        private async Task<bool> CanAccessTaskAsync(TaskItemDto task)
        {
            if (User.IsInRole(ApplicationRoles.Supervisor)) return true;

            var employeeId = TryGetEmployeeIdFromClaims();
            if (!employeeId.HasValue) return false;

            if (User.IsInRole(ApplicationRoles.Employee))
                return task.Executor?.Id == employeeId.Value;

            if (User.IsInRole(ApplicationRoles.ProjectManager))
            {
                var project = await _projectService.GetByIdAsync(task.ProjectId);
                return project is not null && project.ProjectManager?.Id == employeeId.Value;
            }

            return false;
        }

        private async Task<bool> IsEmployeeAsync(Guid employeeId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
            if (user is null) return false;
            return await _userManager.IsInRoleAsync(user, ApplicationRoles.Employee);
        }

        private async Task<bool> IsProjectManagerAsync(Guid employeeId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
            if (user is null) return false;
            return await _userManager.IsInRoleAsync(user, ApplicationRoles.ProjectManager);
        }
    }
}
