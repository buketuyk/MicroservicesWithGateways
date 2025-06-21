using AutoMapper;
using CodeFirstMicroservice.Models;
using CodeFirstMicroservice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using CodeFirstMicroservice.Models.Dtos;
using Castle.Core.Logging;


namespace CodeFirstMicroservice.Tests
{
    public class StatusServiceTests
    {
        private readonly ILogger<StatusService> _logger = Mock.Of<ILogger<StatusService>>();
        private readonly IMapper _mapper = Mock.Of<IMapper>();

        // İzole in-memory context oluşturan yardımcı
        private TaskManagementContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TaskManagementContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TaskManagementContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoStatusExists()
        {
            await using var ctx = CreateInMemoryContext(nameof(GetAllAsync_ShouldReturnEmpty_WhenNoStatusExists));
            var svc = new StatusService(ctx, _logger, _mapper);

            var result = await svc.GetAllAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos_WhenStatusesExist()
        {
            await using var ctx = CreateInMemoryContext(nameof(GetAllAsync_ShouldReturnMappedDtos_WhenStatusesExist));
            var entities = new List<Status>
            {
                new Status { Id = 1, Name = "Active" },
                new Status { Id = 2, Name = "Passive" }
            };
            await ctx.Statuses.AddRangeAsync(entities);
            await ctx.SaveChangesAsync();

            Mock.Get(_mapper)
                .Setup(m => m.Map<IEnumerable<StatusDto>>(It.IsAny<IEnumerable<Status>>()))
                .Returns((IEnumerable<Status> src) =>
                    src.Select(s => new StatusDto { Id = s.Id, Name = s.Name }));

            var svc = new StatusService(ctx, _logger, _mapper);
            var result = (await svc.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.Name == "Active");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await using var ctx = CreateInMemoryContext(nameof(GetByIdAsync_ShouldReturnNull_WhenNotFound));
            var svc = new StatusService(ctx, _logger, _mapper);

            var result = await svc.GetByIdAsync(123);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
        {
            await using var ctx = CreateInMemoryContext(nameof(GetByIdAsync_ShouldReturnDto_WhenFound));
            var entity = new Status { Id = 5, Name = "Test" };
            await ctx.Statuses.AddAsync(entity);
            await ctx.SaveChangesAsync();

            Mock.Get(_mapper)
                .Setup(m => m.Map<StatusDto>(It.IsAny<Status>()))
                .Returns((Status s) => new StatusDto { Id = s.Id, Name = s.Name });

            var svc = new StatusService(ctx, _logger, _mapper);
            var result = await svc.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal("Test", result!.Name);
        }

        [Fact]
        public async Task PostAsync_ShouldAddAndReturnDto()
        {
            await using var ctx = CreateInMemoryContext(nameof(PostAsync_ShouldAddAndReturnDto));
            var dto = new StatusDto { Name = "New" };

            Mock.Get(_mapper)
                .Setup(m => m.Map<Status>(It.IsAny<StatusDto>()))
                .Returns((StatusDto d) => new Status { Name = d.Name });
            Mock.Get(_mapper)
                .Setup(m => m.Map<StatusDto>(It.IsAny<Status>()))
                .Returns((Status s) => new StatusDto { Id = s.Id, Name = s.Name });

            var svc = new StatusService(ctx, _logger, _mapper);
            var result = await svc.PostAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("New", result.Name);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenIdMismatch()
        {
            await using var ctx = CreateInMemoryContext(nameof(UpdateAsync_ShouldReturnFalse_WhenIdMismatch));
            var svc = new StatusService(ctx, _logger, _mapper);
            var dto = new StatusDto { Id = 10, Name = "X" };

            var result = await svc.UpdateAsync(5, dto);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            await using var ctx = CreateInMemoryContext(nameof(UpdateAsync_ShouldReturnFalse_WhenNotFound));
            var svc = new StatusService(ctx, _logger, _mapper);
            var dto = new StatusDto { Id = 5, Name = "X" };

            var result = await svc.UpdateAsync(5, dto);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenUpdated()
        {
            await using var ctx = CreateInMemoryContext(nameof(UpdateAsync_ShouldReturnTrue_WhenUpdated));
            var entity = new Status { Id = 7, Name = "Old" };
            await ctx.Statuses.AddAsync(entity);
            await ctx.SaveChangesAsync();

            var dto = new StatusDto { Id = 7, Name = "Updated" };
            var svc = new StatusService(ctx, _logger, _mapper);

            var result = await svc.UpdateAsync(7, dto);
            Assert.True(result);

            var updated = await ctx.Statuses.FindAsync(7);
            Assert.Equal("Updated", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            await using var ctx = CreateInMemoryContext(nameof(DeleteAsync_ShouldReturnFalse_WhenNotFound));
            var svc = new StatusService(ctx, _logger, _mapper);

            var result = await svc.DeleteAsync(99);
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            await using var ctx = CreateInMemoryContext(nameof(DeleteAsync_ShouldReturnTrue_WhenDeleted));
            var entity = new Status { Id = 8, Name = "ToDelete" };
            await ctx.Statuses.AddAsync(entity);
            await ctx.SaveChangesAsync();

            var svc = new StatusService(ctx, _logger, _mapper);
            var result = await svc.DeleteAsync(8);
            Assert.True(result);

            var exists = await ctx.Statuses.FindAsync(8);
            Assert.Null(exists);
        }
    }
}