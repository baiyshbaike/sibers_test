using SibersProject.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SibersProject.BLL.DTOs.Task
{
    // #DTO_обновления_задачи / #update_task_dto
    public class UpdateTaskItemDto
    {
        [Required]
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Range(1, int.MaxValue)]
        public int Priority { get; set; }

        public TaskItemStatus Status { get; set; }

        public Guid? ExecutorId { get; set; }
    }
}
