using Moq;
using SibersProject.BLL.DTOs.Project;
using SibersProject.BLL.Services;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Filters;
using SibersProject.DAL.Repositories.Interfaces;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.Tests.Services
{
    // #тесты_сервиса_проектов / #project_service_tests
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepoMock;
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _projectRepoMock = new Mock<IProjectRepository>();
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _service = new ProjectService(_projectRepoMock.Object, _employeeRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ReturnsCreatedProject()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var manager = new Employee { Id = managerId, FirstName = "Boss", LastName = "Man", Email = "boss@test.com" };

            var dto = new CreateProjectDto
            {
                Name = "Test Project",
                CustomerCompany = "Customer Co",
                ExecutorCompany = "Executor Co",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                Priority = 1,
                ProjectManagerId = managerId
            };

            _employeeRepoMock.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(manager);
            _projectRepoMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
                .ReturnsAsync((Project p) => p);

            var createdProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                CustomerCompany = dto.CustomerCompany,
                ExecutorCompany = dto.ExecutorCompany,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Priority = dto.Priority,
                ProjectManagerId = managerId,
                ProjectManager = manager,
                ProjectEmployees = new List<ProjectEmployee>()
            };

            _projectRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdProject);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Project");
            _projectRepoMock.Verify(r => r.CreateAsync(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_InvalidDates_ThrowsArgumentException()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            _employeeRepoMock.Setup(r => r.GetByIdAsync(managerId))
                .ReturnsAsync(new Employee { Id = managerId });

            var dto = new CreateProjectDto
            {
                Name = "Bad Dates",
                CustomerCompany = "C",
                ExecutorCompany = "E",
                StartDate = DateTime.Today.AddMonths(1), // Start AFTER end!
                EndDate = DateTime.Today,
                Priority = 1,
                ProjectManagerId = managerId
            };

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Start date*");
        }

        [Fact]
        public async Task CreateAsync_NonExistingManager_ThrowsKeyNotFoundException()
        {
            // Arrange
            _employeeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Employee?)null);

            var dto = new CreateProjectDto
            {
                Name = "Test",
                CustomerCompany = "C",
                ExecutorCompany = "E",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Priority = 1,
                ProjectManagerId = Guid.NewGuid()
            };

            // Act & Assert
            await _service.Invoking(s => s.CreateAsync(dto))
                .Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_WithFilter_CallsRepositoryWithFilter()
        {
            // Arrange
            var filter = new ProjectFilter { Priority = 1, SortBy = "name" };
            _projectRepoMock.Setup(r => r.GetFilteredAsync(filter))
                .ReturnsAsync(new List<Project>());

            // Act
            var result = await _service.GetAllAsync(filter);

            // Assert
            _projectRepoMock.Verify(r => r.GetFilteredAsync(filter), Times.Once);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AddEmployeeAsync_ValidIds_CallsRepository()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId });
            _employeeRepoMock.Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync(new Employee { Id = employeeId });
            _projectRepoMock.Setup(r => r.AddEmployeeAsync(projectId, employeeId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.AddEmployeeAsync(projectId, employeeId);

            // Assert
            _projectRepoMock.Verify(r => r.AddEmployeeAsync(projectId, employeeId), Times.Once);
        }
    }
}
