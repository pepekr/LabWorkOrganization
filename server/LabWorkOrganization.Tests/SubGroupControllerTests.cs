using FluentAssertions;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace LabWorkOrganization.Tests.Controllers;
public class SubGroupControllerTests
{
    [Fact]
    public async Task CreateSubgroup_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var mockService = new Mock<ISubgroupService>();
        var newSubgroup = new SubGroup { Id = "1", Name = "Test Subgroup" };

        mockService.Setup(s => s.CreateSubgroup(It.IsAny<SubGroupCreationalDto>()))
            .ReturnsAsync(Result<SubGroup>.Success(newSubgroup));

        var controller = new SubGroupController(mockService.Object);

        // Act
        var result = await controller.CreateSubgroup(new SubGroupCreationalDto());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().Be(newSubgroup);
    }

    [Fact]
    public async Task CreateSubgroup_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var mockService = new Mock<ISubgroupService>();
        string errorMessage = "Subgroup creation failed";

        mockService.Setup(s => s.CreateSubgroup(It.IsAny<SubGroupCreationalDto>()))
            .ReturnsAsync(Result<SubGroup>.Failure(errorMessage));

        var controller = new SubGroupController(mockService.Object);

        // Act
        var result = await controller.CreateSubgroup(new SubGroupCreationalDto());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest?.Value.Should().Be(errorMessage);
    }
}
