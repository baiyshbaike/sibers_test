using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SibersProject.BLL.DTOs.Auth
{
    // #DTO_регистрации / #register_dto
    // Data required to register a new user account
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        // Optionally link to existing employee / Опционально привязать к существующему сотруднику
        public Guid? EmployeeId { get; set; }
    }
}
