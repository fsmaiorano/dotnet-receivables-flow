using Application.Common.Interfaces;
using Application.Common.Security;
using Application.Common.Security.Jwt;
using Application.UseCases.Account.Commands.AuthenticateAccount;
using Domain.Entities;
using Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Account.Commands;

[TestFixture]
public class AuthenticateAccountUnitTest
{
    private Mock<ILogger<AuthenticateAccountHandler>> _loggerMock;
    private Mock<IDataContext> _contextMock;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<AccessManager> _accessManagerMock;
    private AuthenticateAccountHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<AuthenticateAccountHandler>>();
        _contextMock = new Mock<IDataContext>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>();
        _accessManagerMock = new Mock<AccessManager>();
        _handler = new AuthenticateAccountHandler(_loggerMock.Object, _contextMock.Object, _userManagerMock.Object,
            _accessManagerMock.Object);
    }

    [Test]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var command = new AuthenticateAccountCommand { Email = "test@test.com", Password = "password" };
        var user = new ApplicationUser { Email = "test@test.com" };
        var token = new Token { AccessToken = "token", Expiration = "expiration" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _accessManagerMock.Setup(x => x.ValidateCredentials(It.IsAny<AccountEntity>())).Returns(true);
        _accessManagerMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>())).Returns(token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(token.AccessToken, result.AccessToken);
        Assert.AreEqual(token.Expiration, result.Expiration);
    }

    [Test]
    public async Task Handle_InvalidEmail_ReturnsEmptyResponse()
    {
        // Arrange
        var command = new AuthenticateAccountCommand { Email = "invalid@test.com", Password = "password" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNull(result.AccessToken);
        Assert.IsNull(result.Expiration);
    }

    [Test]
    public async Task Handle_InvalidPassword_ReturnsEmptyResponse()
    {
        // Arrange
        var command = new AuthenticateAccountCommand { Email = "test@test.com", Password = "invalid" };
        var user = new ApplicationUser { Email = "test@test.com" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _accessManagerMock.Setup(x => x.ValidateCredentials(It.IsAny<AccountEntity>())).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNull(result.AccessToken);
        Assert.IsNull(result.Expiration);
    }
}