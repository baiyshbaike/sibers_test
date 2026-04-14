using SibersProject.DAL.Entities.Enums;
using SibersProject.DAL.Entities;
namespace SibersProject.DAL.Entities;

// #задача / #task
// Represents a task within a project, assigned to an employee
public class TaskItem
{
    public Guid Id { get; set; }
    // Task name / Называние задачи
    public string Name { get; set; } = string.Empty;
    // Optional comment / Комментарий
    public string? Comment { get; set; }
    // Task priority / Приоритет задачи
    public int Priority { get; set; }
    // Task status enum / Статус задачи 
    public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;
    // FK to project / Внешний ключ на проект
    public Guid ProjectId { get; set; }
    // Navigation: project / Навигация: проект
    public Project Project { get; set; } = null!;
    // FK to task executor (nullable) / Внешний ключ на исполнителя (может быть null)
    public Guid? ExecutorId { get; set; }
    // Navigation: exevutor / Навигация: исполнитель
    public Employee? Executor { get; set; }
}