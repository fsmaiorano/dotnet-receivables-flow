using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Payable.Commands.UpdatePayable;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Payable.Commands;

[TestFixture]
public class UpdatePayableHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_UpdatesPayable()
    {
        var loggerMock = new Mock<ILogger<UpdatePayableHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new UpdatePayableHandler(loggerMock.Object, contextMock.Object);
        var command = new UpdatePayableCommand
            { Id = Guid.NewGuid(), Value = 100.0f, EmissionDate = DateTime.Now, AssignorId = Guid.NewGuid() };

        contextMock.Setup(c => c.Payables.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PayableEntity
            {
                Value = 0,
                EmissionDate = default,
                AssignorId = default
            });

        var result = await handler.Handle(command, CancellationToken.None);

        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_PayableNotFound_ThrowsNotFoundException()
    {
        var loggerMock = new Mock<ILogger<UpdatePayableHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new UpdatePayableHandler(loggerMock.Object, contextMock.Object);
        var command = new UpdatePayableCommand
            { Id = Guid.NewGuid(), Value = 100.0f, EmissionDate = DateTime.Now, AssignorId = Guid.NewGuid() };

        contextMock.Setup(c => c.Payables.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayableEntity)null!);

        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}