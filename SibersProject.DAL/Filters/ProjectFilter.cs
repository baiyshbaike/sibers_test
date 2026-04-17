using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Filters
{
    // #фильтр_проекта / #project_filter
    // Filter and sorting parametres for project queries
    public class ProjectFilter
    {
        // Filter by start date range / Фильтр по диапазону даты начала
        public DateTime? StartDateFrom {  get; set; }
        public DateTime? StartDateTo { get;set; }

        // Filter by end date range / Фильтр по диапазону даты окончания
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }

        // Filter by exact priority / Фильтр по приоритету
        public int? Priority { get; set; }

        // Sort field: "name", "startDate", "endDate", "priority" / Поле для сортировки
        public string? SortBy { get; set; }

        // Sort direction / Направление сортировки
        public bool SortDescending { get; set; } = false;

        // Scope filters used by access-control layer.
        // Фильтры области видимости для слоя контроля доступа.
        public Guid? ProjectManagerId { get; set; }
        public Guid? AssignedEmployeeId { get; set; }
    }
}
