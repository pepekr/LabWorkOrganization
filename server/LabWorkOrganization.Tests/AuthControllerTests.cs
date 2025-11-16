using FluentAssertions;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LabWorkOrganization.Tests.Controllers
{
    public class AuthControllerTests
    {
        #region Login Tests

        [Fact]
        public async Task Login_ShouldReturnOk_WhenSuccessful()
        {
            var mockService = new Mock<IAuthService>();
            var loginDto = new UserLoginDto { Email = "test@example.com", Password = "password" };
            var tokenDto = new JWTTokenDto { AccessToken = "access", RefreshToken = "refresh" };

            mockService.Setup(s => s.HandleLogin(It.IsAny<UserLoginDto>()))
                       .ReturnsAsync(Result<JWTTokenDto>.Success(tokenDto));

            var controller = new AuthController(mockService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.Login(loginDto);

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new
            {
                Email = loginDto.Email,
                AccessToken = tokenDto.AccessToken,
                RefreshToken = tokenDto.RefreshToken
            });
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenFailed()
        {
            var mockService = new Mock<IAuthService>();
            var loginDto = new UserLoginDto();
            string errorMessage = "Invalid credentials";

            mockService.Setup(s => s.HandleLogin(It.IsAny<UserLoginDto>()))
                       .ReturnsAsync(Result<JWTTokenDto>.Failure(errorMessage));

            var controller = new AuthController(mockService.Object);

            var result = await controller.Login(loginDto);

            result.Should().BeOfType<BadRequestObjectResult>();
            (result as BadRequestObjectResult)!.Value.Should().Be(errorMessage);
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_ShouldReturnOk_WhenSuccessful()
        {
            var mockService = new Mock<IAuthService>();
            var registerDto = new UserRegisterDto { Email = "test@example.com", Password = "password" };
            var tokenDto = new JWTTokenDto { AccessToken = "access", RefreshToken = "refresh" };

            mockService.Setup(s => s.HandleRegistration(It.IsAny<UserRegisterDto>()))
                       .ReturnsAsync(Result<JWTTokenDto>.Success(tokenDto));

            var controller = new AuthController(mockService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.Register(registerDto);

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new
            {
                Email = registerDto.Email,
                AccessToken = tokenDto.AccessToken,
                RefreshToken = tokenDto.RefreshToken
            });
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenFailed()
        {
            var mockService = new Mock<IAuthService>();
            var registerDto = new UserRegisterDto();
            string errorMessage = "User already exists";

            mockService.Setup(s => s.HandleRegistration(It.IsAny<UserRegisterDto>()))
                       .ReturnsAsync(Result<JWTTokenDto>.Failure(errorMessage));

            var controller = new AuthController(mockService.Object);

            var result = await controller.Register(registerDto);

            result.Should().BeOfType<BadRequestObjectResult>();
            (result as BadRequestObjectResult)!.Value.Should().Be(errorMessage);
        }

        #endregion

        #region IsLoggedIn Tests




        #endregion

        #region Logout Tests

        [Fact]
        public void Logout_ShouldReturnOk_AndCallDeleteOnCookies()
        {
            var mockService = new Mock<IAuthService>();

            var context = new DefaultHttpContext();
            var controller = new AuthController(mockService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = context }
            };

            var result = controller.Logout();

            result.Should().BeOfType<OkObjectResult>();

            // Verify cookies deleted by checking headers in DefaultHttpContext
            context.Response.Headers["Set-Cookie"].ToString().Should().Contain("access_token");
            context.Response.Headers["Set-Cookie"].ToString().Should().Contain("refresh_token");
        }

        #endregion
    }
}
