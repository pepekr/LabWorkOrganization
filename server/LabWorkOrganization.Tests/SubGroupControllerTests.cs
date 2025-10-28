using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Application.Common;

public class SubGroupControllerTests
{
	// Ok result expected when subgroup creation succeeds
	[Fact]
	public async Task CreateSubgroup_ShouldReturnOk_WhenSuccess()
	{
		// Arrange: mock returns success
		var mock = new Mock<ISubgroupService>();
		mock.Setup(s => s.CreateSubgroup(It.IsAny<SubGroupCreationalDto>()))
			.ReturnsAsync(Result.Success(new object()));

		var controller = new SubGroupController(mock.Object);

		// Act
		var result = await controller.CreateSubgroup(new SubGroupCreationalDto());

		// Assert
		result.Should().BeOfType<OkObjectResult>();
	}

	// BadRequest expected when creation fails
	[Fact]
	public async Task CreateSubgroup_ShouldReturnBadRequest_WhenFailed()
	{
		// Arrange: mock returns failure
		var mock = new Mock<ISubgroupService>();
		mock.Setup(s => s.CreateSubgroup(It.IsAny<SubGroupCreationalDto>()))
			.ReturnsAsync(Result.Failure<object>("error"));

		var controller = new SubGroupController(mock.Object);

		// Act
		var result = await controller.CreateSubgroup(new SubGroupCreationalDto());

		// Assert
		result.Should().BeOfType<BadRequestObjectResult>();
	}
}
