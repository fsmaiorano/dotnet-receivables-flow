using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Account.Commands.UpdateAccount;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Account.Commands;

[TestFixture]
public class UpdateAccountHandlerUnitTest
{
    private Mock<ILogger<UpdateAccountHandler>> _mockLogger;
    private Mock<IDataContext> _mockDataContext;
    private UpdateAccountHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<UpdateAccountHandler>>();
        _mockDataContext = new Mock<IDataContext>();
        _handler = new UpdateAccountHandler(_mockLogger.Object, _mockDataContext.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_UpdatesAccount()
    {
        var command = new UpdateAccountCommand
        {
            Id = Guid.NewGuid(),
            Name = "New Name",
            PasswordHash = "New PasswordHash",
            Email = "new@test.com",
            Role = RoleEnum.Admin
        };

        var account = new AccountEntity
        {
            Name = "Test",
            PasswordHash = "PasswordHash",
            Email = "test@test.com",
            Role = RoleEnum.User
        };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync(account);

        await _handler.Handle(command, CancellationToken.None);

        _mockDataContext.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_AccountNotFound_ThrowsNotFoundException()
    {
        var command = new UpdateAccountCommand
        {
            Id = Guid.NewGuid(),
            Name = "New Name",
            PasswordHash = "New PasswordHash",
            Email = "email@email.com",
            Role = RoleEnum.Admin
        };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync((AccountEntity)null);

        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ErrorDuringAccountUpdate_ThrowsException()
    {
        var command = new UpdateAccountCommand
        {
            Id = Guid.NewGuid(),
            Name = "New Name",
            PasswordHash = "New PasswordHash",
            Email = "email@email.com",
            Role = RoleEnum.Admin
        };

        var account = new AccountEntity
        {
            Name = "Test",
            PasswordHash = "PasswordHash",
            Email = "test@test.com",
            Role = RoleEnum.User
        };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync(account);
        _mockDataContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Throws<Exception>();

        Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}