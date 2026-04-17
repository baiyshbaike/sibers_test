using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SibersProject.BLL.DTOs.Employee;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Entities.Identity;

namespace SibersProject.API.Controllers
{
    // #контроллер_сотрудников / #employees_controller
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    // Only authenticated users; create/update/delete restricted to Supervisor below
    // Только аутентифицированные; создание/изменение/удаление — только Руководитель (ниже)
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeesController(IEmployeeService employeeService, UserManager<ApplicationUser> userManager)
        {
            _employeeService = employeeService;
            _userManager = userManager;
        }
        /// <summary>Get all employees / Получить всех сотрудников</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        /// <summary>Get employee by ID / Получить сотрудника по ID</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return employee is null ? NotFound() : Ok(employee);
        }

        /// <summary>
        /// Search employees by name or email (AJAX autocomplete)
        /// Поиск сотрудников по имени или email (AJAX автодополнение)
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var results = await _employeeService.SearchAsync(q);
            return Ok(results);
        }

        /// <summary>
        /// Get employees linked to users in a specific role.
        /// Получить сотрудников, привязанных к пользователям определенной роли.
        /// </summary>
        [HttpGet("by-role/{role}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByRole(string role)
        {
            if (role != ApplicationRoles.ProjectManager && role != ApplicationRoles.Employee)
                return BadRequest(new { message = "Only ProjectManager and Employee roles are supported." });

            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var employeeIds = usersInRole
                .Where(u => u.EmployeeId.HasValue)
                .Select(u => u.EmployeeId!.Value)
                .ToHashSet();

            var employees = await _employeeService.GetAllAsync();
            var filtered = employees.Where(e => employeeIds.Contains(e.Id));
            return Ok(filtered);
        }

        /// <summary>Create a new employee / Создать нового сотрудника</summary>
        // Only Supervisor can create/update/delete employees
        // Только Руководитель может создавать/редактировать/удалять сотрудников
        [Authorize(Roles = ApplicationRoles.Supervisor)]
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _employeeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Update employee / Обновить сотрудника</summary>
        // Only Supervisor can create/update/delete employees
        // Только Руководитель может создавать/редактировать/удалять сотрудников
        [Authorize(Roles = ApplicationRoles.Supervisor)]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _employeeService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Delete employee / Удалить сотрудника</summary>
        // Only Supervisor can create/update/delete employees
        // Только Руководитель может создавать/редактировать/удалять сотрудников
        [Authorize(Roles = ApplicationRoles.Supervisor)]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _employeeService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
