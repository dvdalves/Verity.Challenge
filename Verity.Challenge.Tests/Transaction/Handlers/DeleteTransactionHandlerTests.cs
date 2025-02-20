using Application.Transaction.Events;
using Application.Transaction.Handlers;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Verity.Challenge.Tests.Transaction.Handlers;

[TestFixture]
public class DeleteTransactionHandlerTests : IDisposable
{
    private TransactionsDbContext? _dbContext;
    private Mock<IPublishEndpoint>? _publishEndpointMock;
    private DeleteTransaction? _handler;

    [SetUp]
    public void SetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TransactionsDbContext(dbOptions);
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new DeleteTransaction(_dbContext!, _publishEndpointMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Test]
    public async Task Handle_ExistingTransaction_ShouldDeleteTransactionAndPublishEvent()
    {
        // Arrange
        var transaction = TransactionEntity.Create(100.00m, TransactionType.Credit);
        _dbContext!.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteTransaction.DeleteTransactionCommand(transaction.Id);

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var deletedTransaction = await _dbContext.Transactions.FindAsync(transaction.Id);
        deletedTransaction.Should().BeNull();

        _publishEndpointMock!.Verify(x =>
            x.Publish(It.IsAny<TransactionDeleted>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_NonExistingTransaction_ShouldReturnFalse()
    {
        // Arrange
        var command = new DeleteTransaction.DeleteTransactionCommand(Guid.NewGuid());

        // Act
        var result = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        _publishEndpointMock!.Verify(x =>
            x.Publish(It.IsAny<TransactionDeleted>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
