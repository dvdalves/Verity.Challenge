using Application.Transaction.Handlers;
using FluentAssertions;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Enums;
using Shared.Messages;

namespace Transactions.Tests.Handlers;

[TestFixture]
public class CreateTransactionHandlerTests
{
    private Mock<TransactionsDbContext>? _dbContextMock;
    private Mock<IPublishEndpoint>? _publishEndpointMock;
    private CreateTransaction? _handler;

    [SetUp]
    public void SetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContextMock = new Mock<TransactionsDbContext>(dbOptions);
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new CreateTransaction(_dbContextMock.Object, _publishEndpointMock.Object);
    }

    [Test]
    public async Task Handle_ValidTransaction_ShouldCreateTransactionAndPublishEvent()
    {
        // Arrange
        var command = new CreateTransaction.CreateTransactionCommand(100.00m, TransactionType.Credit);

        // Act
        var transactionId = await _handler!.Handle(command, CancellationToken.None);

        // Assert
        transactionId.Should().NotBe(Guid.Empty);

        _publishEndpointMock!.Verify(x =>
            x.Publish(It.IsAny<TransactionCreated>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidAmount_ShouldThrowException()
    {
        // Arrange
        var command = new CreateTransaction.CreateTransactionCommand(-10.00m, TransactionType.Credit);

        // Act
        Func<Task> act = async () => await _handler!.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Amount must be greater than zero.");

        _publishEndpointMock!.Verify(x =>
            x.Publish(It.IsAny<TransactionCreated>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
