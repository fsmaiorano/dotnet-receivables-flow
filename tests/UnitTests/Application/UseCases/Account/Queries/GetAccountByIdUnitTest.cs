using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Account.Queries;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Account.Queries;

[TestFixture]
public class GetAccountByIdHandlerUnitTest
{
    private Mock<ILogger<GetAccountByIdHandler>> _mockLogger;
    private Mock<IDataContext> _mockDataContext;
    private GetAccountByIdHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<GetAccountByIdHandler>>();
        _mockDataContext = new Mock<IDataContext>();
        _handler = new GetAccountByIdHandler(_mockLogger.Object, _mockDataContext.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsAccount()
    {
        var command = new GetAccountByIdQuery { Id = Guid.NewGuid() };
        var account = new AccountEntity
        {
            Name = "Test",
            PasswordHash = "PasswordHash",
            Email = "test@test.com",
            Role = RoleEnum.User
        };


        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result.Account, Is.EqualTo(account));
    }

    [Test]
    public void Handle_AccountNotFound_ThrowsNotFoundException()
    {
        var command = new GetAccountByIdQuery { Id = Guid.NewGuid() };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .ReturnsAsync((AccountEntity)null!);

        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ErrorDuringAccountRetrieval_ThrowsException()
    {
        var command = new GetAccountByIdQuery { Id = Guid.NewGuid() };

        _mockDataContext.Setup(x => x.Accounts.FindAsync(new object[] { command.Id }, CancellationToken.None))
            .Throws<Exception>();

        Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}