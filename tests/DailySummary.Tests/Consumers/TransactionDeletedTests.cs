using Application.Consumers;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Enums;
using Shared.Messages;

namespace DailySummary.Tests.Consumers;

[TestFixture]
public class TransactionDeletedConsumerTests : BaseTests
{
    private TransactionDeletedConsumer _consumer = null!;

    [SetUp]
    public void SetUp()
    {
        _consumer = new TransactionDeletedConsumer(DbContextMock.Object);
    }

    [Test]
    public async Task Consume_WhenTransactionExists_ShouldRemoveTransactionAndUpdateSummary()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date;
        var amount = 150.00m;
        var type = TransactionType.Credit;

        var transaction = DailyTransactionEntity.Create(transactionId, date, amount, type);
        var summary = DailySummaryEntity.Create(date, 150.00m, 50.00m);
        DbContextMock.Object.DailyTransactions.Add(transaction);
        DbContextMock.Object.DailySummaries.Add(summary);
        await DbContextMock.Object.SaveChangesAsync();

        var message = new TransactionDeleted(transactionId);
        var contextMock = new Mock<ConsumeContext<TransactionDeleted>>();
        contextMock.Setup(c => c.Message).Returns(message);

        DbContextMock.Setup(db => db.DailyTransactions.FirstOrDefaultAsync(t => t.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        DbContextMock.Setup(db => db.DailySummaries.FirstOrDefaultAsync(s => s.Date == date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(summary);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        DbContextMock.Verify(db => db.DailyTransactions.Remove(It.IsAny<DailyTransactionEntity>()), Times.Once);
        DbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Consume_WhenTransactionDoesNotExist_ShouldNotChangeDatabase()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var message = new TransactionDeleted(transactionId);
        var contextMock = new Mock<ConsumeContext<TransactionDeleted>>();
        contextMock.Setup(c => c.Message).Returns(message);

        DbContextMock.Setup(db => db.DailyTransactions.FirstOrDefaultAsync(t => t.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DailyTransactionEntity?)null);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        DbContextMock.Verify(db => db.DailyTransactions.Remove(It.IsAny<DailyTransactionEntity>()), Times.Never);
        DbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
