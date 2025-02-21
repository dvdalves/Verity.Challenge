﻿using Application.Transaction.Handlers;
using Domain.Entities;
using Shared.Enums;

namespace Transactions.Tests.Handlers;

[TestFixture]
public class GetTransactionByIdHandlerTests : BaseTests
{
    private GetTransactionById _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new GetTransactionById(DbContextMock.Object, Mapper);
    }

    [Test]
    public async Task Handle_ExistingTransaction_ShouldReturnTransactionDto()
    {
        // Arrange
        var transaction = TransactionEntity.Create(200.00m, TransactionType.Credit);
        DbContextMock.Object.Transactions.Add(transaction);
        await DbContextMock.Object.SaveChangesAsync();

        var query = new GetTransactionById.GetTransactionByIdQuery(transaction.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(transaction.Id));
        Assert.That(result.Amount, Is.EqualTo(transaction.Amount));
        Assert.That(result.Type, Is.EqualTo(transaction.Type));
    }

    [Test]
    public async Task Handle_NonExistingTransaction_ShouldReturnNull()
    {
        // Arrange
        var query = new GetTransactionById.GetTransactionByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}