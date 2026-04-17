using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.DTOs.Auth
{
    // #DTO_ответа_авторизации / #auth_response_dto
    // Response returned after successful login or registration
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
