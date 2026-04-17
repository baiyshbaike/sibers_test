using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SibersProject.BLL.DTOs.Auth;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SibersProject.BLL.Services
{
    // #сервис_авторизации / #auth_service
    // Handles user registration, login and JWT token generation
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpiresHours;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

            // Read JWT config from environment variables / Читаем JWT конфиг из переменных окружения
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "DefaultSecretKeyPleaseChangeInProduction!";
            _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "SibersAPI";
            _jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "SibersClient";
            _jwtExpiresHours = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRES_HOURS"), out var h) ? h : 24;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Validate role / Проверяем роль
            if (!ApplicationRoles.All.Contains(dto.Role))
                throw new ArgumentException($"Invalid role '{dto.Role}'. Valid roles: {string.Join(", ", ApplicationRoles.All)}");

            // Check if user already exists / Проверяем существование пользователя
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser is not null)
                throw new InvalidOperationException($"User with email '{dto.Email}' already exists.");

            // Ensure role exists in DB / Убеждаемся что роль существует в БД
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmployeeId = dto.EmployeeId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            return await GenerateTokenAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return await GenerateTokenAsync(user);
        }

        // Generates JWT token for the user / Генерирует JWT токен для пользователя
        private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email,          user.Email ?? string.Empty),
            new(ClaimTypes.Role,           role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(_jwtExpiresHours);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email ?? string.Empty,
                Role = role,
                ExpiresAt = expires
            };
        }
    }
}
