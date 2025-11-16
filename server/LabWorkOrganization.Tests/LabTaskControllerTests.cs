using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using LabWorkOrganization.API.Controllers.LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Application.Common;

public class LabTaskControllerTests
{
	// Checks Ok result when service returns tasks successfully
	[Fact]
	public async Task GetAllTasks_ShouldReturnOk_WhenSuccess()
	{
		// Arrange
		var mock = new Mock<ILabTaskService>();
		mock.Setup(s => s.GetAllTasksByCourseId(It.IsAny<string>()))
			.ReturnsAsync(Result.Success(new List<LabTaskGetDto>()));

		var controller = new LabTaskController(mock.Object);

		// Act
		var result = await controller.GetAllTaskByCourseId("1");

		// Assert: Ok if success
		result.Should().BeOfType<OkObjectResult>();
	}

	// Checks BadRequest when service returns failure
	[Fact]
	public async Task GetAllTasks_ShouldReturnBadRequest_WhenFailed()
	{
		// Arrange
		var mock = new Mock<ILabTaskService>();
		mock.Setup(s => s.GetAllTasksByCourseId(It.IsAny<string>()))
			.ReturnsAsync(Result.Failure<List<LabTaskGetDto>>("error"));

		var controller = new LabTaskController(mock.Object);

		// Act
		var result = await controller.GetAllTaskByCourseId("1");

		// Assert: BadRequest if failure
		result.Should().BeOfType<BadRequestObjectResult>();
	}
}
