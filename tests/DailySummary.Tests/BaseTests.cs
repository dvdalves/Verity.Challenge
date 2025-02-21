using AutoMapper;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DailySummary.Tests;

public abstract class BaseTests
{
    protected Mock<DailySummaryDbContext> DbContextMock = null!;
    protected IMapper Mapper = null!;

    [SetUp]
    public void BaseSetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<DailySummaryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DailySummaryDbContext(dbOptions);
        DbContextMock = new Mock<DailySummaryDbContext>(dbOptions);
        DbContextMock.Setup(db => db.DailySummaries).Returns(dbContext.DailySummaries);
        DbContextMock.Setup(db => db.DailyTransactions).Returns(dbContext.DailyTransactions);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DailySummaryProfile>();
        });

        Mapper = mapperConfig.CreateMapper();
    }
}