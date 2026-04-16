using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SibersProject.BLL.DTOs.Project
{
    // #DTO_создания_проекта / #create_project_dto
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CustomerCompany { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ExecutorCompany { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Priority must be a positive integer")]
        public int Priority { get; set; }

        [Required]
        public Guid ProjectManagerId { get; set; }
    }
}
