using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Assignor.Commands.UpdateAssignor;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Assignor.Commands;

[TestFixture]
public class UpdateAssignorHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_UpdatesAssignor()
    {
        var loggerMock = new Mock<ILogger<UpdateAssignorHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new UpdateAssignorHandler(loggerMock.Object, contextMock.Object);
        var command = new UpdateAssignorCommand { Id = Guid.NewGuid(), Name = "New Name" };

        contextMock.Setup(c => c.Assignors.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssignorEntity
            {
                Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com"
            });

        var result = await handler.Handle(command, CancellationToken.None);

        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_AssignorNotFound_ThrowsNotFoundException()
    {
        var loggerMock = new Mock<ILogger<UpdateAssignorHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new UpdateAssignorHandler(loggerMock.Object, contextMock.Object);
        var command = new UpdateAssignorCommand { Id = Guid.NewGuid(), Name = "New Name" };

        contextMock.Setup(c => c.Assignors.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssignorEntity)null!);

        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}