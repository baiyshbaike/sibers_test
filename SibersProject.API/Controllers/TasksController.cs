using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SibersProject.BLL.DTOs.Task;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Filters;

namespace SibersProject.API.Controllers
{
    // #контроллер_задач / #tasks_controller
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>Get filtered tasks / Получить отфильтрованные задачи</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] TaskFilter filter)
        {
            var tasks = await _taskService.GetAllAsync(filter);
            return Ok(tasks);
        }

        /// <summary>Get task by ID / Получить задачу по ID</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return task is null ? NotFound() : Ok(task);
        }

        /// <summary>Create new task / Создать новую задачу</summary>
        [HttpPost]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateTaskItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _taskService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Update task / Обновить задачу</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _taskService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Delete task / Удалить задачу</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _taskService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
