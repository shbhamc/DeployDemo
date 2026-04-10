using DeployDemo.Controllers;
using DeployDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace DeployDemoTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetAll_ReturnsAllRecords_WhenCacheIsEmpty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DeployDemoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DeployDemoContext(options);

            context.DeployDemos.Add(new DeployDTO { Id = 1, Name = "Test1" });
            context.DeployDemos.Add(new DeployDTO { Id = 2, Name = "Test2" });
            await context.SaveChangesAsync();

            var cacheMock = new Mock<IDistributedCache>();

            // ✅ FIX: mock GetAsync instead of GetStringAsync
            cacheMock.Setup(x => x.GetAsync("GetAll", default))
                     .ReturnsAsync((byte[])null);

            cacheMock.Setup(x => x.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    default))
                .Returns(Task.CompletedTask);

            var controller = new DeployController(
                context,
                blobService: null,
                loggerFactory: LoggerFactory.Create(builder => { }),
                jwtService: null,
                httpClient: new HttpClient(),
                config: null,
                cache: cacheMock.Object
            );

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<DeployDTO>>(okResult.Value);

            Assert.Equal(2, data.Count);

            // Verify cache set
            cacheMock.Verify(x => x.SetAsync(
                "GetAll",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                default), Times.Once);
        }
    }
}