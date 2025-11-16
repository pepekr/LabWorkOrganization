using FluentAssertions;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace LabWorkOrganization.Tests.Controllers;
public class LabTaskControllerTests
{
    [Fact]
    public async Task GetAllTasks_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var mockService = new Mock<ILabTaskService>();
        mockService.Setup(s => s.GetAllTasksByCourseId(It.IsAny<string>(), false))
            .ReturnsAsync(Result<IEnumerable<LabTask>>.Success(new List<LabTask>()));

        var controller = new LabTaskController(mockService.Object);

        // Act
        var result = await controller.GetAllTaskByCourseId("1");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeAssignableTo<IEnumerable<LabTask>>();
    }


    [Fact]
    public async Task GetAllTasks_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var mockService = new Mock<ILabTaskService>();
        mockService.Setup(s => s.GetAllTasksByCourseId(It.IsAny<string>(), false))
            .ReturnsAsync(Result<IEnumerable<LabTask>>.Failure("error"));

        var controller = new LabTaskController(mockService.Object);

        // Act
        var result = await controller.GetAllTaskByCourseId("1");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest?.Value.Should().Be("error");
    }
}
