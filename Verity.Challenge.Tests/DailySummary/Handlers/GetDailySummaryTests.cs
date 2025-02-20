using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Verity.Challenge.DailySummary.Application.DailySummary.Handlers.GetDailySummary;
using Verity.Challenge.DailySummary.Infrastructure.Persistence;
using Verity.Challenge.DailySummary.Application.DailySummary.Handlers;
using Verity.Challenge.DailySummary.Infrastructure.Configurations;
using Verity.Challenge.DailySummary.Domain.Entities;
using FluentAssertions;

namespace Verity.Challenge.Tests.DailySummary.Handlers;

[TestFixture]
public class GetDailySummaryHandlerTests
{
    private DailySummaryDbContext _dbContext;
    private IMapper _mapper;
    private GetDailySummary _handler;

    [SetUp]
    public void SetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<DailySummaryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Banco em memória
            .Options;

        _dbContext = new DailySummaryDbContext(dbOptions);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DailySummaryProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _handler = new GetDailySummary(_dbContext, _mapper);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Handle_ExistingSummary_ShouldReturnDailySummaryDto()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;
        var summary = DailySummaryEntity.Create(date, 500.00m, 200.00m);
        _dbContext.DailySummaries.Add(summary);
        await _dbContext.SaveChangesAsync();

        var query = new GetDailySummaryQuery(date);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Date.Should().Be(date);
        result.TotalCredits.Should().Be(summary.TotalCredits);
        result.TotalDebits.Should().Be(summary.TotalDebits);
        result.Balance.Should().Be(summary.TotalCredits - summary.TotalDebits);
    }

    [Test]
    public async Task Handle_NonExistingSummary_ShouldReturnNull()
    {
        // Arrange
        var query = new GetDailySummaryQuery(DateTime.UtcNow.Date);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}