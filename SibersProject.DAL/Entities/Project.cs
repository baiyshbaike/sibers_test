using SibersProject.DAL.Entities;
using SibersProject.DAL.Entities.Enums;
namespace SibersProject.DAL.Entities;

// #проект / #project
// Represents a project with customer, contractor, team and timeline
public class Project
{
    public Guid Id { get; set; }
    // Project name / Название проекта
    public string Name { get; set; } = string.Empty;
    // Customer company name / Название компании-заказчика
    public string CustomerCompany { get; set; } = string.Empty;
    // Contractor company name / Название компании-исполнителя
    public string ExecutorCompany { get; set; } = string.Empty; 
    // Project start date / Дата начала проекта
    public DateTime StartDate { get; set; }
    // Project and date / Дата окончания проекта 
    public DateTime EndDate { get; set; }
    // Project priority (integer, higher = more priority) / Приоритет проекта
    public int Priority { get; set; }
    // FK to the project manager / Внешний ключ на руководителя проекта
    public Guid ProjectManagerId { get; set; }
    // Navigation: project manager / Навигация: руководитель проекта
    public Employee ProjectManager { get; set; } = null!;
    // Navigation: many-to-many Employees / Навигация: сотрудники проекта 
    public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    // Navigation: project tasks / Навигация: задачи проекта
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    // Navigation: project documents / Навигация: документы проекта
    public ICollection<ProjectDocument> Documents { get; set; } = new List<ProjectDocument>();
}