using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Assignor.Commands.DeleteAssignor;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Assignor.Commands;

[TestFixture]
public class DeleteAssignorHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_DeletesAssignor()
    {
        var loggerMock = new Mock<ILogger<DeleteAssignorHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new DeleteAssignorHandler(loggerMock.Object, contextMock.Object);
        var command = new DeleteAssignorCommand { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Assignors.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssignorEntity
            {
                Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com"
            });

        var result = await handler.Handle(command, CancellationToken.None);

        contextMock.Verify(c => c.Assignors.Remove(It.IsAny<AssignorEntity>()), Times.Once);
        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_AssignorNotFound_ThrowsNotFoundException()
    {
        var loggerMock = new Mock<ILogger<DeleteAssignorHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new DeleteAssignorHandler(loggerMock.Object, contextMock.Object);
        var command = new DeleteAssignorCommand { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Assignors.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssignorEntity)null!);

        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}