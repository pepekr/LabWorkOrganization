using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Common;

public class AuthControllerTests
{
	// Verifies controller returns Ok when login succeeds
	[Fact]
	public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
	{
		// Arrange: mock service returns a successful login result
		var mock = new Mock<IAuthService>();
		mock.Setup(s => s.HandleLogin(It.IsAny<UserLoginDto>()))
			.ReturnsAsync(Result.Success(new TokenResponse { AccessToken = "a", RefreshToken = "r" }));

		var controller = new AuthController(mock.Object);

		// Act
		var result = await controller.Login(new UserLoginDto());

		// Assert: should be HTTP 200 (Ok)
		result.Should().BeOfType<OkObjectResult>();
	}

	// Verifies controller returns BadRequest when login fails
	[Fact]
	public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
	{
		// Arrange: mock service returns failed result
		var mock = new Mock<IAuthService>();
		mock.Setup(s => s.HandleLogin(It.IsAny<UserLoginDto>()))
			.ReturnsAsync(Result.Failure<TokenResponse>("error"));

		var controller = new AuthController(mock.Object);

		// Act
		var result = await controller.Login(new UserLoginDto());

		// Assert: should be HTTP 400 (BadRequest)
		result.Should().BeOfType<BadRequestObjectResult>();
	}
}
