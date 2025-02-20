using Application.Transaction.Handlers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Tests.Handlers;

[TestFixture]
public class GetTransactionsHandlerTests
{
    private TransactionsDbContext? _dbContext;
    private IMapper? _mapper;
    private GetTransactions? _handler;

    [SetUp]
    public void SetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TransactionsDbContext(dbOptions);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TransactionProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _handler = new GetTransactions(_dbContext, _mapper);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Handle_WhenTransactionsExist_ShouldReturnTransactionDtos()
    {
        // Arrange
        var transaction1 = TransactionEntity.Create(200.00m, TransactionType.Credit);
        var transaction2 = TransactionEntity.Create(150.00m, TransactionType.Debit);
        _dbContext.Transactions.AddRange(transaction1, transaction2);
        await _dbContext.SaveChangesAsync();

        var query = new GetTransactions.GetTransactionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().ContainSingle(t => t.Id == transaction1.Id && t.Amount == transaction1.Amount);
        result.Should().ContainSingle(t => t.Id == transaction2.Id && t.Amount == transaction2.Amount);
    }

    [Test]
    public async Task Handle_WhenNoTransactionsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetTransactions.GetTransactionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}