using SibersProject.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Filters
{
    // #фильтр_задач / #task_filter
    // Filter and sorting parameters for task queries
    public class TaskFilter
    {
        // Filter by project / Фильтр по проекту
        public Guid? ProjectId { get; set; }

        // Filter by status / Фильтр по статусу
        public TaskItemStatus? Status { get; set; }

        // Filter by executor / Фильтр по исполнителю
        public Guid? ExecutorId { get; set; }

        // Scope filter for manager own projects / Фильтр области для проектов менеджера
        public Guid? ProjectManagerId { get; set; }

        // Sort field: "name", "priority", "status", Поле для сортировки
        public string? SortBy { get; set; }

        // Sort direction / Направление сортировки
        public bool SortDescending { get; set; }
    }
}
