using Application.Consumers;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Enums;
using Shared.Messages;

namespace DailySummary.Tests.Consumers;

[TestFixture]
public class TransactionUpdatedConsumerTests : BaseTests
{
    private TransactionUpdatedConsumer _consumer = null!;

    [SetUp]
    public void SetUp()
    {
        _consumer = new TransactionUpdatedConsumer(DbContext);
    }

    [Test]
    public async Task Consume_WhenSummaryExists_ShouldUpdateSummary()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow.Date;
        var amount = 200.00m;
        var type = TransactionType.Debit;

        var summary = DailySummaryEntity.Create(updatedAt, 500.00m, 300.00m);
        DbContext.DailySummaries.Add(summary);
        await DbContext.SaveChangesAsync();

        var message = new TransactionUpdated(Guid.NewGuid(), amount, type, updatedAt);
        var contextMock = new Mock<ConsumeContext<TransactionUpdated>>();
        contextMock.Setup(c => c.Message).Returns(message);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        var updatedSummary = await DbContext.DailySummaries.FirstOrDefaultAsync(s => s.Date == updatedAt);
        Assert.That(updatedSummary, Is.Not.Null);
        Assert.That(updatedSummary!.TotalDebits, Is.EqualTo(500.00m));
        Assert.That(updatedSummary.TotalCredits, Is.EqualTo(500.00m));
    }

    [Test]
    public async Task Consume_WhenSummaryDoesNotExist_ShouldDoNothing()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow.Date;
        var amount = 300.00m;
        var type = TransactionType.Credit;

        var message = new TransactionUpdated(Guid.NewGuid(), amount, type, updatedAt);
        var contextMock = new Mock<ConsumeContext<TransactionUpdated>>();
        contextMock.Setup(c => c.Message).Returns(message);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        var summaryCount = await DbContext.DailySummaries.CountAsync();
        Assert.That(summaryCount, Is.EqualTo(0));
    }
}
