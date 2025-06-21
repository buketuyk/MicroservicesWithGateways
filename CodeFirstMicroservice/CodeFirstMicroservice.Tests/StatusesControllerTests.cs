using CodeFirstMicroservice.Controllers;
using CodeFirstMicroservice.Interfaces;
using CodeFirstMicroservice.Models.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeFirstMicroservice.Tests
{
    public class StatusesControllerTests
    {
        private readonly Mock<IStatusService> _serviceMock;
        private readonly ILogger<StatusesController> _logger = Mock.Of<ILogger<StatusesController>>();
        private readonly StatusesController _controller;

        public StatusesControllerTests()
        {
            _serviceMock = new Mock<IStatusService>();
            _controller = new StatusesController(_logger, _serviceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNotFound_WhenEmpty()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(Enumerable.Empty<StatusDto>());

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No statuses found.", notFound.Value);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk_WithData()
        {
            // Arrange
            var list = new List<StatusDto> { new StatusDto { Id = 1, Name = "A" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var actionResult = await _controller.GetAllAsync();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnList = Assert.IsAssignableFrom<IEnumerable<StatusDto>>(ok.Value);
            Assert.Single(returnList);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetByIdAsync(42))
                .ReturnsAsync((StatusDto?)null);

            // Act
            var result = await _controller.GetByIdAsync(42);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFound.Value);

            var valueType = notFound.Value.GetType();
            var prop = valueType.GetProperty("message");
            Assert.NotNull(prop);  // Özelliğin gerçekten var olduğundan emin ol

            var message = prop.GetValue(notFound.Value) as string;
            Assert.Equal("Status with ID 42 not found.", message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOk_WithDto()
        {
            // Arrange
            var dto = new StatusDto { Id = 5, Name = "Test" };
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetByIdAsync(5);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.PostAsync(new StatusDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtRoute_WithDto()
        {
            // Arrange
            var dto = new StatusDto { Id = 7, Name = "New" };
            _serviceMock.Setup(s => s.PostAsync(It.IsAny<StatusDto>()))
                        .ReturnsAsync(dto);

            // Act
            var result = await _controller.PostAsync(new StatusDto { Name = "New" });

            // Assert
            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal(nameof(StatusesController.GetByIdAsync), created.RouteName);
            Assert.Equal(7, created.RouteValues["id"]);
            Assert.Equal(dto, created.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateAsync(1, new StatusDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadRequest_WhenIdMismatch()
        {
            // Act
            var result = await _controller.UpdateAsync(2, new StatusDto { Id = 3, Name = "X" });

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.NotNull(bad.Value);
            var anonType = bad.Value.GetType();
            var prop = anonType.GetProperty("message");
            Assert.NotNull(prop);

            var message = prop.GetValue(bad.Value) as string;
            Assert.Equal("Route ID does not match Status ID.", message);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenServiceFalse()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(4, It.IsAny<StatusDto>()))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateAsync(4, new StatusDto { Id = 4, Name = "X" });

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);
            var anonType = notFound.Value.GetType();
            var prop = anonType.GetProperty("message");
            Assert.NotNull(prop);

            var message = prop.GetValue(notFound.Value) as string;
            Assert.Equal("Status with ID 4 does not exist.", message);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenServiceTrue()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateAsync(5, It.IsAny<StatusDto>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(5, new StatusDto { Id = 5, Name = "X" });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenServiceFalse()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(9)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAsync(9);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);

            // through Reflection
            var valueType = notFound.Value!.GetType();
            var prop = valueType.GetProperty("message");
            Assert.NotNull(prop);

            var message = prop.GetValue(notFound.Value) as string;
            Assert.Equal("Status with ID 9 not found.", message);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenServiceTrue()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(8)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAsync(8);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
