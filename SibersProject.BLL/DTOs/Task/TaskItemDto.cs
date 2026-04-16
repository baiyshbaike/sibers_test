using SibersProject.BLL.DTOs.Employee;
using SibersProject.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.DTOs.Task
{
    // #DTO_задачи / #task_item_dto
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public TaskItemStatus Status { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public EmployeeDto? Author { get; set; }
        public EmployeeDto? Executor { get; set; }
    }
}
