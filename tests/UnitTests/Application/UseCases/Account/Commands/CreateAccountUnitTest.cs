using Application.Common.Interfaces;
using Application.UseCases.Account.Commands.CreateAccount;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Account.Commands;

[TestFixture]
public class CreateAccountHandlerUnitTest
{
    [Test]
    public async Task Handle_ValidRequest_CreatesAccount()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateAccountHandler>>();
        var contextMock = new Mock<IDataContext>();
        var mockSet = new Mock<DbSet<AccountEntity>>();
        contextMock.Setup(c => c.Accounts).Returns(mockSet.Object);
        var handler = new CreateAccountHandler(loggerMock.Object, contextMock.Object);
        var command = new CreateAccountCommand
            { Name = "Test", Email = "test@test.com", Role = RoleEnum.Admin, PasswordHash = "123456" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        contextMock.Verify(c => c.Accounts.Add(It.IsAny<AccountEntity>()), Times.Once);
        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Handle_ErrorDuringCreation_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateAccountHandler>>();
        var contextMock = new Mock<IDataContext>();
        contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<Exception>();
        var handler = new CreateAccountHandler(loggerMock.Object, contextMock.Object);
        var command = new CreateAccountCommand
            { Name = "Test", Email = "test@test.com", Role = RoleEnum.Admin, PasswordHash = "123456" };

        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}