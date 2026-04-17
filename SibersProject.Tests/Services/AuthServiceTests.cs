using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SibersProject.BLL.DTOs.Auth;
using SibersProject.BLL.Services;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SibersProject.Tests.Services;

// #тесты_сервиса_аутентификации / #auth_service_tests
public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
        _configurationMock = new Mock<IConfiguration>();

        var jwtSection = new Dictionary<string, string>
        {
            {"Jwt:Secret", "TestSecretKey123456789012345678901234567890"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"},
            {"Jwt:ExpirationHours", "24"}
        };

        _configurationMock.Setup(c => c[It.IsAny<string>()]).Returns((string key) => jwtSection.GetValueOrDefault(key));

        _authService = new AuthService(_userManagerMock.Object, _signInManagerMock.Object, 
            _roleManagerMock.Object, _configurationMock.Object);
    }

    [Fact]
    // #регистрация_успешная / #register_success
    public async Task RegisterAsync_ValidData_ReturnsSuccess()
    {
        // Arrange / Подготовка
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = "Employee"
        };

        var user = new ApplicationUser { Email = registerDto.Email, UserName = registerDto.Email };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act / Действие
        var result = await _authService.RegisterAsync(registerDto);

        // Assert / Проверка
        result.Should().NotBeNull();
        result.Email.Should().Be(registerDto.Email);
        result.Role.Should().Be(registerDto.Role);
        result.Token.Should().NotBeNullOrEmpty();
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    // #регистрация_ошибка / #register_error
    public async Task RegisterAsync_InvalidPassword_ReturnsError()
    {
        // Arrange / Подготовка
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "123", // Too short
            Role = "Employee"
        };

        var identityError = new IdentityError { Description = "Password too short" };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act & Assert / Действие и проверка
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));
        exception.Message.Should().Contain("Password too short");
    }

    [Fact]
    // #вход_успешный / #login_success
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange / Подготовка
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new ApplicationUser 
        { 
            Email = loginDto.Email, 
            UserName = loginDto.Email,
            Id = Guid.NewGuid()
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new[] { "Employee" });
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Success);

        // Act / Действие
        var result = await _authService.LoginAsync(loginDto);

        // Assert / Проверка
        result.Should().NotBeNull();
        result.Email.Should().Be(loginDto.Email);
        result.Role.Should().Be("Employee");
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(24), TimeSpan.FromMinutes(1));
    }

    [Fact]
    // #вход_неверный_пароль / #login_wrong_password
    public async Task LoginAsync_WrongPassword_ThrowsException()
    {
        // Arrange / Подготовка
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = new ApplicationUser 
        { 
            Email = loginDto.Email, 
            UserName = loginDto.Email 
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act & Assert / Действие и проверка
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
        exception.Message.Should().Contain("Invalid credentials");
    }

    [Fact]
    // #вход_пользователь_не_найден / #login_user_not_found
    public async Task LoginAsync_UserNotFound_ThrowsException()
    {
        // Arrange / Подготовка
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act & Assert / Действие и проверка
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
        exception.Message.Should().Contain("Invalid credentials");
    }

    [Fact]
    // #генерация_токена / #token_generation
    public void GenerateJwtToken_ValidUser_ReturnsValidToken()
    {
        // Arrange / Подготовка
        var user = new ApplicationUser 
        { 
            Email = "test@example.com", 
            UserName = "test@example.com",
            Id = Guid.NewGuid()
        };
        var roles = new[] { "Employee" };

        // Act / Действие
        var token = _authService.GenerateJwtToken(user, roles);

        // Assert / Проверка
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        
        jsonToken.Should().NotBeNull();
        jsonToken!.Issuer.Should().Be("TestIssuer");
        jsonToken.Audiences.Should().Contain("TestAudience");
        jsonToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Employee");
        jsonToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddHours(24), TimeSpan.FromMinutes(1));
    }

    [Fact]
    // #регистрация_с_сотрудником / #register_with_employee
    public async Task RegisterAsync_WithEmployeeId_Success()
    {
        // Arrange / Подготовка
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = "Employee",
            EmployeeId = Guid.NewGuid()
        };

        var user = new ApplicationUser { Email = registerDto.Email, UserName = registerDto.Email };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act / Действие
        var result = await _authService.RegisterAsync(registerDto);

        // Assert / Проверка
        result.Should().NotBeNull();
        result.Email.Should().Be(registerDto.Email);
        result.Role.Should().Be(registerDto.Role);
        _userManagerMock.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u => u.EmployeeId == registerDto.EmployeeId), 
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    // #регистрация_несуществующая_роль / #register_nonexistent_role
    public async Task RegisterAsync_NonExistentRole_CreatesRole()
    {
        // Arrange / Подготовка
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = "NewRole"
        };

        var user = new ApplicationUser { Email = registerDto.Email, UserName = registerDto.Email };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(x => x.RoleExistsAsync(registerDto.Role))
            .ReturnsAsync(false);
        _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act / Действие
        var result = await _authService.RegisterAsync(registerDto);

        // Assert / Проверка
        result.Should().NotBeNull();
        result.Role.Should().Be(registerDto.Role);
        _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerDto.Role), Times.Once);
    }
}
