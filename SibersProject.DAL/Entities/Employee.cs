namespace SibersProject.DAL.Entities;

//#сотрудни / #employee
//Represents a company employee who can be assgined to projects and tasks
public class Employee
{
    public Guid Id { get; set; }
    // First name / Имя
    public string FirstName { get; set; }
    // Last name / Фамилия
    public string LastName { get; set; }
    // Middle name / Отчество
    public string? MiddleName { get; set; }
    // Email address / Электронная почта
    public string Email { get; set; }
    // Projects where this employee is the manager / Проекты где является руководилетям
    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    // Projects this employee is assigned to / Проекты в каторых участыует сотрудник
    public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    // Tasks wuthored by this employee / Задача создание сотрудником
    public ICollection<TaskItem> AuthoredTasks { get; set; } = new List<TaskItem>();
    // Tasks assigned to this employee / Задача назначение сотруднику 
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}