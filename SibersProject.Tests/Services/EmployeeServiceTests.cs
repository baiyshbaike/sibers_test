using Moq;
using SibersProject.BLL.DTOs.Employee;
using SibersProject.BLL.Services;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Repositories.Interfaces;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.Tests.Services
{
    // #тесты_сервиса_сотрудников / #employee_service_tests
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _repoMock;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _service = new EmployeeService(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEmployees()
        {
            // Arrange / Подготовка
            var employees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Ivan", LastName = "Petrov", Email = "ivan@test.com" },
            new() { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Ivanova", Email = "anna@test.com" }
        };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

            // Act / Действие
            var result = await _service.GetAllAsync();

            // Assert / Проверка
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Email == "ivan@test.com");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEmployee()
        {
            // Arrange
            var id = Guid.NewGuid();
            var employee = new Employee { Id = id, FirstName = "Ivan", LastName = "Petrov", Email = "ivan@test.com" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(employee);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("ivan@test.com");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            // Act
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_UniqueEmail_CreatesSuccessfully()
        {
            // Arrange
            var dto = new CreateEmployeeDto
            {
                FirstName = "Ivan",
                LastName = "Petrov",
                Email = "ivan@test.com"
            };

            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null)).ReturnsAsync(false);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
                .ReturnsAsync((Employee e) => e);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("ivan@test.com");
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateEmployeeDto { FirstName = "Ivan", LastName = "Petrov", Email = "dup@test.com" };
            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null)).ReturnsAsync(true);

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already exists*");
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(Guid.NewGuid()))
                .Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
