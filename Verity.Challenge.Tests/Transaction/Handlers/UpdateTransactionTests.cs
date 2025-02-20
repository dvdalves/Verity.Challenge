using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Verity.Challenge.Transactions.Application.Transaction.Events;
using Verity.Challenge.Transactions.Application.Transaction.Handlers;
using Verity.Challenge.Transactions.Domain.Entities;
using Verity.Challenge.Transactions.Domain.Enums;
using Verity.Challenge.Transactions.Infrastructure.Persistence;

namespace Verity.Challenge.Tests.Transaction.Handlers;

[TestFixture]
public class UpdateTransactionHandlerTests
{
    private TransactionsDbContext? _dbContextMock;
    private Mock<IPublishEndpoint>? _publishEndpointMock;
    private UpdateTransaction? _handler;

    [SetUp]
    public void SetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContextMock = new TransactionsDbContext(dbOptions);
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new UpdateTransaction(_dbContextMock, _publishEndpointMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContextMock.Dispose();
    }

    [Test]
    public async Task Handle_ExistingTransaction_ShouldUpdateTransactionAndPublishEvent()
    {
        // Arrange
        var transaction = TransactionEntity.Create(100.00m, TransactionType.Credit);
        _dbContextMock.Transactions.Add(transaction);
        await _dbContextMock.SaveChangesAsync();

        var command = new UpdateTransaction.UpdateTransactionCommand(transaction.Id, 250.00m, TransactionType.Debit);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        var updatedTransaction = await _dbContextMock.Transactions.FindAsync(transaction.Id);
        updatedTransaction.Should().NotBeNull();
        updatedTransaction.Amount.Should().Be(250.00m);
        updatedTransaction.Type.Should().Be(TransactionType.Debit);
        updatedTransaction.UpdatedAt.Should().NotBeNull();

        _publishEndpointMock.Verify(x =>
            x.Publish(It.IsAny<TransactionUpdated>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_NonExistingTransaction_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateTransaction.UpdateTransactionCommand(Guid.NewGuid(), 300.00m, TransactionType.Credit);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        _publishEndpointMock.Verify(x =>
            x.Publish(It.IsAny<TransactionUpdated>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}