using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class TodoControllerTests
{
    private readonly Mock<ITodoRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _mockRepository = new Mock<ITodoRepository>();
        _mockMapper = new Mock<IMapper>();
        _controller = new TodoController(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkResult_WithListOfTodoItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Item 1", Description = "Description 1" },
            new TodoItem { Id = 2, Title = "Item 2", Description = "Description 2" }
        };
        var todoItemDTOs = new List<TodoItemDTO>
        {
            new TodoItemDTO { Id = 1, Title = "Item 1", Description = "Description 1" },
            new TodoItemDTO { Id = 2, Title = "Item 2", Description = "Description 2" }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDTO>>(todoItems)).Returns(todoItemDTOs);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(todoItemDTOs);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsOkResult_WithTodoItem()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "Item 1", Description = "Description 1" };
        var todoItemDTO = new TodoItemDTO { Id = 1, Title = "Item 1", Description = "Description 1" };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);
        _mockMapper.Setup(mapper => mapper.Map<TodoItemDTO>(todoItem)).Returns(todoItemDTO);

        // Act
        var result = await _controller.GetByIdAsync(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(todoItemDTO);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNotFoundResult()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((TodoItem)null);

        // Act
        var result = await _controller.GetByIdAsync(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}