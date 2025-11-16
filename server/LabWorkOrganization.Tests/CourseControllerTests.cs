using FluentAssertions;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LabWorkOrganization.Tests.Controllers
{
    public class CourseControllerTests
    {
        #region V1 Tests

        [Fact]
        public async Task V1_GetAllCourses_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            var expectedCourses = new List<Course>();
            mockService.Setup(s => s.GetAllCoursesByUserAsync(It.IsAny<bool>()))
                       .ReturnsAsync(Result<IEnumerable<Course>>.Success(expectedCourses));

            var controller = new CourseControllerV1(mockService.Object);

            // Act
            var result = await controller.GetAllCourses();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedCourses);
        }

        [Fact]
        public async Task V1_GetAllCourses_ShouldReturnBadRequest_WhenFailed()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            string errorMessage = "Something went wrong";
            mockService.Setup(s => s.GetAllCoursesByUserAsync(It.IsAny<bool>()))
                       .ReturnsAsync(Result<IEnumerable<Course>>.Failure(errorMessage));

            var controller = new CourseControllerV1(mockService.Object);

            // Act
            var result = await controller.GetAllCourses();

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badResult = result as BadRequestObjectResult;
            badResult!.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task V1_GetCourseById_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            var course = new Course { Id = "123", Name = "Test Course" };
            mockService.Setup(s => s.GetCourseById(It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(Result<Course?>.Success(course));

            var controller = new CourseControllerV1(mockService.Object);

            // Act
            var result = await controller.GetCourseById("123", false);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult)!.Value.Should().Be(course);
        }

        [Fact]
        public async Task V1_GetCourseById_ShouldReturnBadRequest_WhenFailed()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            string errorMessage = "Course not found";
            mockService.Setup(s => s.GetCourseById(It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(Result<Course?>.Failure(errorMessage));

            var controller = new CourseControllerV1(mockService.Object);

            // Act
            var result = await controller.GetCourseById("123", false);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            (result as BadRequestObjectResult)!.Value.Should().Be(errorMessage);
        }

        #endregion

        #region V2 Tests

        [Fact]
        public async Task V2_GetAllCourses_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            var courses = new List<Course>();
            mockService.Setup(s => s.GetAllCoursesByUserAsyncV2(It.IsAny<bool>()))
                       .ReturnsAsync(Result<IEnumerable<Course>>.Success(courses));

            var controller = new CourseControllerV2(mockService.Object);

            // Act
            var result = await controller.GetAllCourses();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(courses);
        }

        [Fact]
        public async Task V2_GetCourseById_ShouldReturnBadRequest_WhenFailed()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            string errorMessage = "Course not found";
            mockService.Setup(s => s.GetCourseByIdV2(It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(Result<Course?>.Failure(errorMessage));

            var controller = new CourseControllerV2(mockService.Object);

            // Act
            var result = await controller.GetCourseById("abc", false);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            (result as BadRequestObjectResult)!.Value.Should().Be(errorMessage);
        }

        #endregion
    }
}
