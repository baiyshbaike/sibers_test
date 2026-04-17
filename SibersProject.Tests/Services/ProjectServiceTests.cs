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
                .ReturnsAsync(new Employee { Id = managerId, FirstName = "Manager", LastName = "User", Email = "manager@test.com" });

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
                .ReturnsAsync(new Employee { Id = employeeId, FirstName = "Emp", LastName = "User", Email = "emp@test.com" });
            _projectRepoMock.Setup(r => r.AddEmployeeAsync(projectId, employeeId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.AddEmployeeAsync(projectId, employeeId);

            // Assert
            _projectRepoMock.Verify(r => r.AddEmployeeAsync(projectId, employeeId), Times.Once);
        }

        [Fact]
        // #обновление_проекта_успешное / #update_project_success
        public async Task UpdateAsync_ValidData_ReturnsSuccess()
        {
            // Arrange / Подготовка
            var projectId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var manager = new Employee { Id = managerId, FirstName = "Boss", LastName = "Man", Email = "boss@test.com" };
            var existingProject = new Project 
            { 
                Id = projectId, 
                Name = "Old Name",
                CustomerCompany = "Old Customer",
                ExecutorCompany = "Old Executor",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                Priority = 1,
                ProjectManagerId = managerId
            };

            var updateDto = new UpdateProjectDto
            {
                Name = "Updated Project",
                CustomerCompany = "Updated Customer",
                ExecutorCompany = "Updated Executor",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddMonths(4),
                Priority = 2,
                ProjectManagerId = managerId
            };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(existingProject);
            _employeeRepoMock.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(manager);
            _projectRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

            // Act / Действие
            await _service.UpdateAsync(projectId, updateDto);

            // Assert / Проверка
            _projectRepoMock.Verify(r => r.UpdateAsync(It.Is<Project>(p => 
                p.Name == updateDto.Name && 
                p.CustomerCompany == updateDto.CustomerCompany &&
                p.ExecutorCompany == updateDto.ExecutorCompany &&
                p.StartDate == updateDto.StartDate &&
                p.EndDate == updateDto.EndDate &&
                p.Priority == updateDto.Priority)), Times.Once);
        }

        [Fact]
        // #удаление_проекта_успешное / #delete_project_success
        public async Task DeleteAsync_ExistingProject_CallsRepository()
        {
            // Arrange / Подготовка
            var projectId = Guid.NewGuid();
            var existingProject = new Project 
            { 
                Id = projectId, 
                Name = "Test Project",
                CustomerCompany = "Customer",
                ExecutorCompany = "Executor",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                Priority = 1
            };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(existingProject);
            _projectRepoMock.Setup(r => r.DeleteAsync(projectId)).Returns(Task.CompletedTask);

            // Act / Действие
            await _service.DeleteAsync(projectId);

            // Assert / Проверка
            _projectRepoMock.Verify(r => r.DeleteAsync(projectId), Times.Once);
        }

        [Fact]
        // #удаление_сотрудника_успешное / #remove_employee_success
        public async Task RemoveEmployeeAsync_ValidIds_CallsRepository()
        {
            // Arrange / Подготовка
            var projectId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId });
            _employeeRepoMock.Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync(new Employee { Id = employeeId, FirstName = "Emp", LastName = "User", Email = "emp@test.com" });
            _projectRepoMock.Setup(r => r.RemoveEmployeeAsync(projectId, employeeId))
                .Returns(Task.CompletedTask);

            // Act / Действие
            await _service.RemoveEmployeeAsync(projectId, employeeId);

            // Assert / Проверка
            _projectRepoMock.Verify(r => r.RemoveEmployeeAsync(projectId, employeeId), Times.Once);
        }

        [Fact]
        // #фильтрация_по_датам / #filter_by_dates
        public async Task GetAllAsync_WithDateFilter_CallsRepositoryWithCorrectFilter()
        {
            // Arrange / Подготовка
            var filter = new ProjectFilter 
            { 
                StartDateFrom = DateTime.Today,
                StartDateTo = DateTime.Today.AddDays(30),
                EndDateFrom = DateTime.Today.AddMonths(2),
                EndDateTo = DateTime.Today.AddMonths(5),
                Priority = 1,
                SortBy = "startDate",
                SortDescending = true
            };
            _projectRepoMock.Setup(r => r.GetFilteredAsync(filter))
                .ReturnsAsync(new List<Project>());

            // Act / Действие
            var result = await _service.GetAllAsync(filter);

            // Assert / Проверка
            _projectRepoMock.Verify(r => r.GetFilteredAsync(It.Is<ProjectFilter>(f => 
                f.StartDateFrom == filter.StartDateFrom &&
                f.StartDateTo == filter.StartDateTo &&
                f.EndDateFrom == filter.EndDateFrom &&
                f.EndDateTo == filter.EndDateTo &&
                f.Priority == filter.Priority &&
                f.SortBy == filter.SortBy &&
                f.SortDescending == filter.SortDescending)), Times.Once);
            result.Should().BeEmpty();
        }

        [Fact]
        // #добавление_документов_успешное / #add_documents_success
        public async Task AddDocumentsAsync_ValidData_CallsRepository()
        {
            // Arrange / Подготовка
            var projectId = Guid.NewGuid();
            var documents = new List<ProjectDocument>
            {
                new ProjectDocument 
                { 
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FileName = "doc1.pdf",
                    StoredFileName = "stored_doc1.pdf",
                    ContentType = "application/pdf",
                    Size = 1024,
                    UploadedAtUtc = DateTime.UtcNow
                },
                new ProjectDocument 
                { 
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FileName = "doc2.docx",
                    StoredFileName = "stored_doc2.docx",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    Size = 2048,
                    UploadedAtUtc = DateTime.UtcNow
                }
            };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId });
            _projectRepoMock.Setup(r => r.AddDocumentsAsync(documents))
                .Returns(Task.CompletedTask);

            // Act / Действие
            await _service.AddDocumentsAsync(projectId, documents);

            // Assert / Проверка
            _projectRepoMock.Verify(r => r.AddDocumentsAsync(documents), Times.Once);
        }

        [Fact]
        // #удаление_документа_успешное / #delete_document_success
        public async Task DeleteDocumentAsync_ValidIds_CallsRepository()
        {
            // Arrange / Подготовка
            var projectId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var document = new ProjectDocument 
            { 
                Id = documentId,
                ProjectId = projectId,
                FileName = "test.pdf",
                StoredFileName = "stored_test.pdf",
                ContentType = "application/pdf",
                Size = 1024,
                UploadedAtUtc = DateTime.UtcNow
            };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId });
            _projectRepoMock.Setup(r => r.GetDocumentAsync(projectId, documentId))
                .ReturnsAsync(document);
            _projectRepoMock.Setup(r => r.RemoveDocumentAsync(document))
                .Returns(Task.CompletedTask);

            // Act / Действие
            await _service.DeleteDocumentAsync(projectId, documentId);

            // Assert / Проверка
            _projectRepoMock.Verify(r => r.GetDocumentAsync(projectId, documentId), Times.Once);
            _projectRepoMock.Verify(r => r.RemoveDocumentAsync(document), Times.Once);
        }
    }
}
