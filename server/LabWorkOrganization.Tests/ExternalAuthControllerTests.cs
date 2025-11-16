using FluentAssertions;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
namespace LabWorkOrganization.Tests.Controllers;
public class ExternalAuthControllerTests
{
    [Fact]
    public void ExternalLogin_ShouldReturnRedirect()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.RedirectUri()).Returns("http://redirect.url");

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = controller.ExternalLogin("returnUrl");

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirect = result as RedirectResult;
        redirect?.Url.Should().Be("http://redirect.url");
    }

    [Fact]
    public async Task ExternalCallback_ShouldReturnRedirect_WhenSuccess()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.HandleExternalAuth(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(LabWorkOrganization.Domain.Utilities.Result<JWTTokenDto>.Success(
                new JWTTokenDto { AccessToken = "a", RefreshToken = "r" }
            ));

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = await controller.ExternalCallback("code");

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirect = result as RedirectResult;
        redirect?.Url.Should().Contain("http://localhost:4200?linked=true");
    }

    [Fact]
    public async Task ExternalCallback_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.HandleExternalAuth(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(LabWorkOrganization.Domain.Utilities.Result<JWTTokenDto>.Failure("error"));

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = await controller.ExternalCallback("code");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest?.Value.Should().Be("error");
    }

    [Fact]
    public async Task ExternalLogout_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.HandleExternalLogout())
            .ReturnsAsync(LabWorkOrganization.Domain.Utilities.Result<string>.Success("logged out"));

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = await controller.ExternalLogout();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok?.Value.Should().Be("logged out");
    }

    [Fact]
    public async Task ExternalLogout_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.HandleExternalLogout())
            .ReturnsAsync(LabWorkOrganization.Domain.Utilities.Result<string>.Failure("fail"));

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = await controller.ExternalLogout();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest?.Value.Should().Be("fail");
    }

    [Fact]
    public async Task IsLoggedIn_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.IsLoggedIn())
            .ReturnsAsync(LabWorkOrganization.Domain.Utilities.Result<bool>.Success(true));

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = await controller.IsLoggedIn();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok?.Value.Should().Be(true);
    }

    [Fact]
    public async Task IsLoggedIn_ShouldReturnUnauthorized_WhenFailed()
    {
        // Arrange
        var mockService = new Mock<IExternalAuthService>();
        mockService.Setup(s => s.IsLoggedIn())
            .ReturnsAsync(LabWorkOrganization.Domain.Utilities.Result<bool>.Failure("fail"));

        var controller = new ExternalAuthController(mockService.Object);

        // Act
        var result = await controller.IsLoggedIn();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorized = result as UnauthorizedObjectResult;
        unauthorized?.Value.Should().Be("fail");
    }
}
