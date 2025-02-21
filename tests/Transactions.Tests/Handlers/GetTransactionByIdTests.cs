using Application.Transaction.Handlers;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Transactions.Tests.Handlers;

[TestFixture]
public class GetTransactionByIdHandlerTests
{
    private TransactionsDbContext? _dbContext;
    private IMapper? _mapper;
    private GetTransactionById? _handler;

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

        _handler = new GetTransactionById(_dbContext, _mapper);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Handle_ExistingTransaction_ShouldReturnTransactionDto()
    {
        // Arrange
        var transaction = TransactionEntity.Create(200.00m, TransactionType.Credit);
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        var query = new GetTransactionById.GetTransactionByIdQuery(transaction.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(transaction.Id);
        result.Amount.Should().Be(transaction.Amount);
        result.Type.Should().Be(transaction.Type);
    }

    [Test]
    public async Task Handle_NonExistingTransaction_ShouldReturnNull()
    {
        // Arrange
        var query = new GetTransactionById.GetTransactionByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}