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
        _consumer = new TransactionCreatedConsumer(DbContext);
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

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        var createdTransaction = await DbContext.DailyTransactions.FirstOrDefaultAsync(t => t.Id == transactionId);
        Assert.That(createdTransaction, Is.Not.Null);
        Assert.That(createdTransaction!.Amount, Is.EqualTo(amount));
        Assert.That(createdTransaction.Type, Is.EqualTo(type));

        var createdSummary = await DbContext.DailySummaries.FirstOrDefaultAsync();
        Assert.That(createdSummary, Is.Not.Null);
    }

    [Test]
    public async Task Consume_WhenTransactionAlreadyExists_ShouldNotCreateOrUpdate()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var amount = 100.00m;
        var type = TransactionType.Debit;

        var existingTransaction = DailyTransactionEntity.Create(transactionId, createdAt, amount, type);

        DbContext.DailyTransactions.Add(existingTransaction);
        await DbContext.SaveChangesAsync();

        var message = new TransactionCreated(transactionId, amount, type, createdAt);
        var contextMock = new Mock<ConsumeContext<TransactionCreated>>();
        contextMock.Setup(c => c.Message).Returns(message);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        var transactionCount = await DbContext.DailyTransactions.CountAsync();
        Assert.That(transactionCount, Is.EqualTo(1));

        var summaryCount = await DbContext.DailySummaries.CountAsync();
        Assert.That(summaryCount, Is.EqualTo(0));
    }
}