using Application.DailySummary.Handlers;
using Domain.Entities;
using static Application.DailySummary.Handlers.GetDailySummary;

namespace DailySummary.Tests.Handlers;

[TestFixture]
public class GetDailySummaryHandlerTests : BaseTests
{
    private GetDailySummary _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new GetDailySummary(DbContext, Mapper);
    }

    [Test]
    public async Task Handle_ExistingSummary_ShouldReturnDailySummaryDto()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;
        var summary = DailySummaryEntity.Create(date, 500.00m, 200.00m);
        DbContext.DailySummaries.Add(summary);
        await DbContext.SaveChangesAsync();

        var query = new GetDailySummaryQuery(date);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Date, Is.EqualTo(date));
            Assert.That(result.TotalCredits, Is.EqualTo(summary.TotalCredits));
            Assert.That(result.TotalDebits, Is.EqualTo(summary.TotalDebits));
            Assert.That(result.Balance, Is.EqualTo(summary.TotalCredits - summary.TotalDebits));
        });
    }

    [Test]
    public async Task Handle_NonExistingSummary_ShouldReturnNull()
    {
        // Arrange
        var query = new GetDailySummaryQuery(DateTime.UtcNow.Date);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}