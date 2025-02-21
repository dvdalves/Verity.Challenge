using Application.Consumers;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Enums;
using Shared.Messages;

namespace DailySummary.Tests.Consumers;

[TestFixture]
public class TransactionCreatedConsumerTests : BaseTests
{
    private TransactionCreatedConsumer _consumer = null!;

    [SetUp]
    public void SetUp()
    {
        _consumer = new TransactionCreatedConsumer(DbContextMock.Object);
    }

    [Test]
    public async Task Consume_WhenTransactionDoesNotExist_ShouldCreateTransactionAndUpdateSummary()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var amount = 150.00m;
        var type = TransactionType.Credit;

        var message = new TransactionCreated(transactionId, amount, type, createdAt);
        var contextMock = new Mock<ConsumeContext<TransactionCreated>>();
        contextMock.Setup(c => c.Message).Returns(message);

        DbContextMock.Setup(db => db.DailyTransactions.AnyAsync(t => t.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        DbContextMock.Verify(db => db.DailyTransactions.Add(It.IsAny<DailyTransactionEntity>()), Times.Once);
        DbContextMock.Verify(db => db.DailySummaries.Add(It.IsAny<DailySummaryEntity>()), Times.Once);
        DbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Consume_WhenTransactionAlreadyExists_ShouldNotCreateOrUpdate()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var amount = 100.00m;
        var type = TransactionType.Debit;

        var message = new TransactionCreated(transactionId, amount, type, createdAt);
        var contextMock = new Mock<ConsumeContext<TransactionCreated>>();
        contextMock.Setup(c => c.Message).Returns(message);

        DbContextMock.Setup(db => db.DailyTransactions.AnyAsync(t => t.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        DbContextMock.Verify(db => db.DailyTransactions.Add(It.IsAny<DailyTransactionEntity>()), Times.Never);
        DbContextMock.Verify(db => db.DailySummaries.Add(It.IsAny<DailySummaryEntity>()), Times.Never);
        DbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}