using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Common;

public class ExternalAuthControllerTests
{
	// Ok expected when external auth callback succeeds
	[Fact]
	public async Task ExternalCallback_ShouldReturnOk_WhenSuccess()
	{
		// Arrange: simulate external login success
		var mock = new Mock<IExternalAuthService>();
		mock.Setup(s => s.HandleExternalAuth(It.IsAny<string>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
			.ReturnsAsync(Result.Success("ok"));

		var controller = new ExternalAuthController(mock.Object);

		// Act
		var result = await controller.ExternalCallback("code");

		// Assert: OkObjectResult when successful
		result.Should().BeOfType<OkObjectResult>();
	}

	// BadRequest expected when callback fails
	[Fact]
	public async Task ExternalCallback_ShouldReturnBadRequest_WhenFailed()
	{
		// Arrange: simulate failed authentication
		var mock = new Mock<IExternalAuthService>();
		mock.Setup(s => s.HandleExternalAuth(It.IsAny<string>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
			.ReturnsAsync(Result.Failure<string>("error"));

		var controller = new ExternalAuthController(mock.Object);

		// Act
		var result = await controller.ExternalCallback("code");

		// Assert
		result.Should().BeOfType<BadRequestObjectResult>();
	}
}
