namespace SibersProject.DAL.Entities;

//#сотрудник / #employee
//Represents a company employee who can be assigned to projects and tasks
public class Employee
{
    public Guid Id { get; set; }
    // First name / Имя
    public required string FirstName { get; set; }
    // Last name / Фамилия
    public required string LastName { get; set; }
    // Middle name / Отчество
    public string? MiddleName { get; set; }
    // Email address / Электронная почта
    public required string Email { get; set; }
    // Projects where this employee is the manager / Проекты где является руководителем
    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    // Projects this employee is assigned to / Проекты в которых участвует сотрудник
    public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    // Tasks authored by this employee / Задачи, созданные сотрудником
    public ICollection<TaskItem> AuthoredTasks { get; set; } = new List<TaskItem>();
    // Tasks assigned to this employee / Задача назначение сотруднику 
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}