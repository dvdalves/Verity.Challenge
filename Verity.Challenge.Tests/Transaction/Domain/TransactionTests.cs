namespace Verity.Challenge.Tests.Transaction.Domain;

[TestFixture]
public class TransactionTests
{
    [Test]
    public void CreateTransaction_ValidData_ShouldCreateTransaction()
    {
        // Arrange
        decimal amount = 100.00m;
        var type = TransactionType.Credit;

        // Act
        var transaction = TransactionEntity.Create(amount, type);

        // Assert
        Assert.NotNull(transaction);
        Assert.That(transaction.Amount, Is.EqualTo(amount));
        Assert.That(transaction.Type, Is.EqualTo(type));
        Assert.That(transaction.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.LessOrEqual(transaction.CreatedAt, DateTime.UtcNow);
    }

    [Test]
    public void CreateTransaction_InvalidAmount_ShouldThrowException()
    {
        // Arrange
        decimal amount = -50.00m; // Valor inválido
        var type = TransactionType.Debit;

        // Act & Assert
        var ex = Assert.Throws<TransactionDomainException>(() => TransactionEntity.Create(amount, type));
        Assert.That(ex.Message, Is.EqualTo("Amount must be greater than zero."));
    }

    [Test]
    public void UpdateTransaction_ValidData_ShouldUpdateTransaction()
    {
        // Arrange
        var transaction = TransactionEntity.Create(50.00m, TransactionType.Debit);
        decimal newAmount = 200.00m;
        var newType = TransactionType.Credit;

        // Act
        transaction.Update(newAmount, newType);

        // Assert
        Assert.That(transaction.Amount, Is.EqualTo(newAmount));
        Assert.That(transaction.Type, Is.EqualTo(newType));
        Assert.NotNull(transaction.UpdatedAt);
    }

    [Test]
    public void UpdateTransaction_InvalidAmount_ShouldThrowException()
    {
        // Arrange
        var transaction = TransactionEntity.Create(100.00m, TransactionType.Credit);
        decimal invalidAmount = 0.00m;

        // Act & Assert
        var ex = Assert.Throws<TransactionDomainException>(() => transaction.Update(invalidAmount, TransactionType.Debit));
        Assert.That(ex.Message, Is.EqualTo("Amount must be greater than zero."));
    }
}