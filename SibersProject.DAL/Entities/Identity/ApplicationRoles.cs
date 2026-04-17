using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Entities.Identity
{
    // #роли / #application_roles
    // Constants for role names used across the application
    public static class ApplicationRoles
    {
        // Supervisor: full access to everything / Руководитель: полный доступ
        public const string Supervisor = "Supervisor";

        // ProjectManager: manage own projects and tasks / Менеджер проекта: управление своими проектами
        public const string ProjectManager = "ProjectManager";

        // Employee: view own projects/tasks, change task status / Сотрудник: просмотр и смена статуса
        public const string Employee = "Employee";

        // All roles as array / Все роли в виде массива
        public static readonly string[] All = [Supervisor, ProjectManager, Employee];
    }
}
