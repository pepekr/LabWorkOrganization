using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using LabWorkOrganization.API.Controllers;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Application.Common;

public class CourseControllerTests
{
	// Verifies Ok when GetAllCourses returns success
	[Fact]
	public async Task GetAllCourses_ShouldReturnOk_WhenSuccess()
	{
		// Arrange: mock service returns a successful list
		var mock = new Mock<ICourseService>();
		mock.Setup(s => s.GetAllCourses())
			.ReturnsAsync(Result.Success(new List<CourseDto>()));

		var controller = new CourseController(mock.Object);

		// Act
		var result = await controller.GetAllCourses();

		// Assert: expect OkObjectResult
		result.Should().BeOfType<OkObjectResult>();
	}

	// Verifies BadRequest when GetAllCourses fails
	[Fact]
	public async Task GetAllCourses_ShouldReturnBadRequest_WhenFailed()
	{
		// Arrange: mock failure response
		var mock = new Mock<ICourseService>();
		mock.Setup(s => s.GetAllCourses())
			.ReturnsAsync(Result.Failure<List<CourseDto>>("error"));

		var controller = new CourseController(mock.Object);

		// Act
		var result = await controller.GetAllCourses();

		// Assert: expect BadRequestObjectResult
		result.Should().BeOfType<BadRequestObjectResult>();
	}
}
