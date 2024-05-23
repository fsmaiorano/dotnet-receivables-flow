using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Account.Commands.DeleteAccount;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Account.Commands;

[TestFixture]
public class DeleteAccountHandlerUnitTest
{
    private Mock<ILogger<DeleteAccountHandler>> _mockLogger;
    private Mock<IDataContext> _mockDataContext;
    private DeleteAccountHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<DeleteAccountHandler>>();
        _mockDataContext = new Mock<IDataContext>();
        _handler = new DeleteAccountHandler(_mockLogger.Object, _mockDataContext.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_DeletesAccount()
    {
        var command = new DeleteAccountCommand { Id = Guid.NewGuid() };
        var account = new AccountEntity
        {
            Id = command.Id,
            Name = "Test",
            PasswordHash = "PasswordHash",
            Email = "test@test.com",
            Role = RoleEnum.User
        };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync(account);

        await _handler.Handle(command, CancellationToken.None);

        _mockDataContext.Verify(x => x.Accounts.Remove(account), Times.Once);
        _mockDataContext.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_AccountNotFound_ThrowsNotFoundException()
    {
        var command = new DeleteAccountCommand { Id = Guid.NewGuid() };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync((AccountEntity)null);

        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ErrorDuringAccountDeletion_ThrowsException()
    {
        var command = new DeleteAccountCommand { Id = Guid.NewGuid() };
        var account = new AccountEntity
        {
            Id = command.Id,
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