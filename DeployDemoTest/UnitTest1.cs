using DeployDemo.Controllers;
using DeployDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DeployDemoTest
{
    public class UnitTest1
    {
        //[Fact]
        //public void Test1()
        //{

        //}
        [Fact]
        public async Task GetAll_ReturnsAllRecords()
        {
            // Arrange: In-memory DB
            var options = new DbContextOptionsBuilder<DeployDemoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DeployDemoContext(options);

            // Add test data
            context.DeployDemos.Add(new DeployDTO { Id = 1, Name = "Test1" });
            context.DeployDemos.Add(new DeployDTO { Id = 2, Name = "Test2" });
            await context.SaveChangesAsync();

            // Create controller (minimal dependencies)
            var controller = new DeployController(
                context,
                blobService: null,
                loggerFactory: LoggerFactory.Create(builder => { }),
                jwtService: null,
                httpClient: new HttpClient(),
                config: null
            );

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsAssignableFrom<List<DeployDTO>>(okResult.Value);

            Assert.Equal(2, data.Count);
        }
    }
}