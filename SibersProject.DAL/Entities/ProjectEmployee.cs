namespace SibersProject.DAL.Entities;

// #проект_сотрудник / #project_employee
// Junction table for many-to-many relationship between Project and Employee
public class ProjectEmployee
{
    // FK to project / Внешний ключ на проект
    public Guid ProjectId { get; set; }
    // Navigation: project / Навигация: проекта
    public Project Project { get; set; } = null!;
    // FK to employee / Внешний ключ на сотрудника 
    public Guid EmployeeId { get; set; }
    // Navigation: employee / Навигация: сотрудник
    public Employee Employee { get; set; } = null!;
}