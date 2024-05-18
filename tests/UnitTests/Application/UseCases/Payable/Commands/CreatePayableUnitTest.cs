using Application.Common.Interfaces;
using Application.UseCases.Payable.Commands.CreatePayable;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Payable.Commands;

[TestFixture]
public class CreatePayableHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_CreatesPayable()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreatePayableHandler>>();
        var contextMock = new Mock<IDataContext>();
        var mockSetAssignor = new Mock<DbSet<AssignorEntity>>();
        contextMock.Setup(c => c.Assignors).Returns(mockSetAssignor.Object);
        var mockSetPayable = new Mock<DbSet<PayableEntity>>();
        contextMock.Setup(c => c.Payables).Returns(mockSetPayable.Object);
        var handler = new CreatePayableHandler(loggerMock.Object, contextMock.Object);
        var command = new CreatePayableCommand
        {
            Value = 100.0f, EmissionDate = DateTime.Now, AssignorId = Guid.NewGuid(), Assignor = new AssignorEntity
            {
                Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com"
            }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        contextMock.Verify(c => c.Payables.Add(It.IsAny<PayableEntity>()), Times.Once);
        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_ErrorDuringCreation_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreatePayableHandler>>();
        var contextMock = new Mock<IDataContext>();
        contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<Exception>();
        var mockSetAssignor = new Mock<DbSet<AssignorEntity>>();
        contextMock.Setup(c => c.Assignors).Returns(mockSetAssignor.Object);
        var mockSetPayable = new Mock<DbSet<PayableEntity>>();
        contextMock.Setup(c => c.Payables).Returns(mockSetPayable.Object);
        var handler = new CreatePayableHandler(loggerMock.Object, contextMock.Object);
        var command = new CreatePayableCommand
        {
            Value = 100.0f, EmissionDate = DateTime.Now, AssignorId = Guid.NewGuid(), Assignor = new AssignorEntity
            {
                Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com"
            }
        };

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await handler.Handle(command, CancellationToken.None));
    }
}