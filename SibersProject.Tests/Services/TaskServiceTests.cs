using Moq;
using SibersProject.BLL.DTOs.Task;
using SibersProject.BLL.Services;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Entities.Enums;
using SibersProject.DAL.Filters;
using SibersProject.DAL.Repositories.Interfaces;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.Tests.Services
{
    // #тесты_сервиса_задач / #task_service_tests
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly Mock<IProjectRepository> _projectRepoMock;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _projectRepoMock = new Mock<IProjectRepository>();
            _service = new TaskService(_taskRepoMock.Object, _employeeRepoMock.Object, _projectRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ReturnsCreatedTask()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var authorId = Guid.NewGuid();

            var dto = new CreateTaskItemDto
            {
                Name = "Fix Bug #42",
                Priority = 2,
                Status = TaskItemStatus.ToDo,
                ProjectId = projectId,
                AuthorId = authorId
            };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId, Name = "Test Project" });
            _employeeRepoMock.Setup(r => r.GetByIdAsync(authorId))
                .ReturnsAsync(new Employee { Id = authorId });
            _taskRepoMock.Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem t) => t);

            var detailedTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Priority = dto.Priority,
                Status = dto.Status,
                ProjectId = projectId,
                AuthorId = authorId,
                Project = new Project { Id = projectId, Name = "Test Project" },
                Author = new Employee { Id = authorId, FirstName = "Ivan", LastName = "Petrov" }
            };

            _taskRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(detailedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Fix Bug #42");
            result.Status.Should().Be(TaskItemStatus.ToDo);
        }

        [Fact]
        public async Task UpdateAsync_ChangeStatus_UpdatesCorrectly()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = taskId,
                Name = "Old Name",
                Status = TaskItemStatus.ToDo,
                Priority = 1
            };

            var updateDto = new UpdateTaskItemDto
            {
                Name = "New Name",
                Status = TaskItemStatus.InProgress,
                Priority = 2
            };

            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem t) => t);

            var updatedTask = new TaskItem
            {
                Id = taskId,
                Name = "New Name",
                Status = TaskItemStatus.InProgress,
                Priority = 2,
                Project = new Project { Name = "P" },
                Author = new Employee { FirstName = "A", LastName = "B" }
            };

            _taskRepoMock.Setup(r => r.GetWithDetailsAsync(taskId)).ReturnsAsync(updatedTask);

            // Act
            var result = await _service.UpdateAsync(taskId, updateDto);

            // Assert
            result.Status.Should().Be(TaskItemStatus.InProgress);
            result.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task GetAllAsync_FilterByStatus_CallsRepositoryCorrectly()
        {
            // Arrange
            var filter = new TaskFilter { Status = TaskItemStatus.Done };
            _taskRepoMock.Setup(r => r.GetFilteredAsync(filter))
                .ReturnsAsync(new List<TaskItem>());

            // Act
            var result = await _service.GetAllAsync(filter);

            // Assert
            _taskRepoMock.Verify(r => r.GetFilteredAsync(filter), Times.Once);
        }
    }
}
