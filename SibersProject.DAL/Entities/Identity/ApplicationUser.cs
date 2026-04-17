using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Entities.Identity
{
    // #пользователь / #application_user
    // Identity user extended with optional link to Employee entity
    public class ApplicationUser : IdentityUser
    {
        // Optional link to Employee entity / Опциональная связь с сущностью сотрудника
        public Guid? EmployeeId { get; set; }

        // Navigation: linked employee / Навигация: связанный сотрудник
        public Employee? Employee { get; set; }
    }
}
