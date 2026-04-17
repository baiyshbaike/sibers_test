using SibersProject.BLL.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services.Interfaces
{
    // #интерфейс_сервиса_авторизации / #auth_service_interface
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
