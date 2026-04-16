using SibersProject.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Filters
{
    // #фильтр_задач / #task_tilter
    // Filter and sorting parameters for task queries
    public class TaskFilter
    {
        // Filter by project / Фильтр по проекту
        public Guid? ProjectId { get; set; }

        // Filter by status / Фильтр по статусу
        public TaskItemStatus? Status { get; set; }

        // Filter by exector / Фильтр по исполнитею
        public Guid? ExecutorId { get; set; }

        // Sort field: "name", "priority", "status", Поле для сортировки
        public string? SortBy { get; set; }

        // Sort direction / Напрваление сортировки
        public bool SortDescending { get; set; }
    }
}
