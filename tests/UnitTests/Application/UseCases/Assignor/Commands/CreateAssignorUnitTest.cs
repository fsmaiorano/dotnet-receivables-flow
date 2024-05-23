using Application.Common.Interfaces;
using Application.UseCases.Assignor.Commands.CreateAssignor;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Assignor.Commands;

[TestFixture]
public class CreateAssignorHandlerUnitTest
{
    [Test]
    public async Task Handle_ValidRequest_CreatesAssignor()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateAssignorHandler>>();
        var contextMock = new Mock<IDataContext>();
        var mockSet = new Mock<DbSet<AssignorEntity>>();
        contextMock.Setup(c => c.Assignors).Returns(mockSet.Object);
        var handler = new CreateAssignorHandler(loggerMock.Object, contextMock.Object);
        var command = new CreateAssignorCommand
            { Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        contextMock.Verify(c => c.Assignors.Add(It.IsAny<AssignorEntity>()), Times.Once);
        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Handle_ErrorDuringCreation_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateAssignorHandler>>();
        var contextMock = new Mock<IDataContext>();
        contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<Exception>();
        var handler = new CreateAssignorHandler(loggerMock.Object, contextMock.Object);
        var command = new CreateAssignorCommand
            { Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com" };

        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}