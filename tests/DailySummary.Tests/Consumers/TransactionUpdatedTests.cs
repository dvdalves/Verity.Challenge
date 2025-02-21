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
        _consumer = new TransactionUpdatedConsumer(DbContextMock.Object);
    }

    [Test]
    public async Task Consume_WhenSummaryExists_ShouldUpdateSummary()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow.Date;
        var amount = 200.00m;
        var type = TransactionType.Debit;

        var summary = DailySummaryEntity.Create(updatedAt, 500.00m, 300.00m);
        DbContextMock.Object.DailySummaries.Add(summary);
        await DbContextMock.Object.SaveChangesAsync();

        var message = new TransactionUpdated(Guid.NewGuid(), amount, type, updatedAt);
        var contextMock = new Mock<ConsumeContext<TransactionUpdated>>();
        contextMock.Setup(c => c.Message).Returns(message);

        DbContextMock.Setup(db => db.DailySummaries.FirstOrDefaultAsync(s => s.Date == updatedAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(summary);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        Assert.That(summary.TotalDebits, Is.EqualTo(500.00m));
        Assert.That(summary.TotalCredits, Is.EqualTo(500.00m));

        DbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

        DbContextMock.Setup(db => db.DailySummaries.FirstOrDefaultAsync(s => s.Date == updatedAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DailySummaryEntity?)null);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        DbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
